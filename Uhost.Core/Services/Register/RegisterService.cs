using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using System;
using System.Net;
using System.Threading.Tasks;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Razor;
using Uhost.Core.Models.User;
using Uhost.Core.Properties;
using Uhost.Core.Services.Email;
using Uhost.Core.Services.Razor;
using Uhost.Core.Services.RedisSwitcher;
using Uhost.Core.Services.Scheduler;
using Uhost.Core.Services.User;

namespace Uhost.Core.Services.Register
{
    /// <summary>
    /// Регистрация
    /// </summary>
    public sealed class RegisterService : BaseService, IRegisterService
    {
        private readonly IRedisSwitcherService _redis;
        private readonly IUserService _users;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IEmailService _email;
        private readonly IRazorService _razor;
        private readonly ISchedulerService _schedule;
        private static readonly TimeSpan _redisKeyTtl = TimeSpan.FromMinutes(30);

        public RegisterService(
            IServiceProvider provider,
            IRedisSwitcherService redis,
            IEmailService email,
            IUserService users,
            IRazorService razor,
            ISchedulerService schedule) : base(provider)
        {
            _contextAccessor = provider.GetService<IHttpContextAccessor>();
            _redis = redis;
            _email = email;
            _users = users;
            _razor = razor;
            _schedule = schedule;
        }

        private static string GetRedisKey(string code, IPAddress ip)
        {
            return $"regquery_{ip}_{code}";
        }

        /// <summary>
        /// Проверка существования логина/почты
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UserExists(UserRegisterModel model)
        {
            return _users.Exists(model.Login, model.Email);
        }

        /// <summary>
        /// Отправка письма с подтверждением регистрации
        /// </summary>
        /// <param name="key">Ключ Redis</param>
        /// <returns></returns>
        public async Task SendRegistrationEmail(string key)
        {
            var value = await _redis.ExecuteAsync(async e => await e.StringGetAsync(key));

            if (!value.TryCastTo<RegistrationRazorDataModel>(out var dataModel))
            {
                var exception = new ApplicationException($"Failed to cast registration model");
                exception.Data["Key"] = key;
                exception.Data["Value"] = value;

                SentrySdk.CaptureException(exception);

                throw exception;
            }

            var html = await _razor.RenderToStringAsync(RazorService.Templates.Registration, dataModel);
            _email.Send(CoreSettings.SmtpConfig.Sender, dataModel.Model.Email, dataModel.Title, html, true);
        }

        /// <summary>
        /// Первый этап - запрос на регистрацию
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task RequestRegistrationAsync(UserRegisterModel model)
        {
            var code = new Random().Next(999999).ToString().PadLeft(6);
            var key = GetRedisKey(code, _contextAccessor?.HttpContext?.ResolveClientIp());
            var dataModel = new RegistrationRazorDataModel
            {
                Title = CoreStrings.Template_Registration_Title.Format(model.Login),
                Code = code,
                Model = model
            };

            await _redis.ExecuteAsync(async e => await e.StringSetAsync(key, dataModel.ToJson(), _redisKeyTtl));

            _schedule.ScheduleRegistrationEmailSend(key);
        }

        /// <summary>
        /// Второй этап - подтверждение регистрации
        /// </summary>
        /// <param name="code">Код из email</param>
        /// <returns></returns>
        public async Task<UserViewModel> ConfirmRegistration(string code)
        {
            var key = GetRedisKey(code, _contextAccessor?.HttpContext?.ResolveClientIp());
            var value = await _redis.ExecuteAsync(async e => await e.StringGetAsync(key));

            if (value.IsNull || !value.HasValue || !value.TryCastTo<UserRegisterModel>(out var model))
            {
                return null;
            }

            var entity = _users.Add(model);

            if (entity != null)
            {
                await _redis.ExecuteAsync(async e => await e.KeyDeleteAsync(key));

                return _users.GetOne(entity.Id);
            }

            return null;
        }
    }
}
