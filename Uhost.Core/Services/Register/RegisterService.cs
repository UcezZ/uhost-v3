using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Net;
using System.Threading.Tasks;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Razor;
using Uhost.Core.Models.User;
using Uhost.Core.Properties;
using Uhost.Core.Services.Email;
using Uhost.Core.Services.Razor;
using Uhost.Core.Services.User;

namespace Uhost.Core.Services.Register
{
    /// <summary>
    /// Регистрация
    /// </summary>
    public sealed class RegisterService : IRegisterService
    {
        private readonly IRedisDatabase _redis;
        private readonly IUserService _users;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IEmailService _email;
        private readonly IRazorService _razor;
        private static readonly TimeSpan _redisKeyTtl = TimeSpan.FromMinutes(30);

        public RegisterService(IServiceProvider provider, IRedisDatabase redis, IEmailService email, IUserService users, IRazorService razor)
        {
            _contextAccessor = provider.GetService<IHttpContextAccessor>();
            _redis = redis;
            _email = email;
            _users = users;
            _razor = razor;
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
        /// Первый этап - запрос на регистрацию
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> RequestRegistrationAsync(UserRegisterModel model)
        {
            var code = new Random().Next(999999).ToString().PadLeft(6);
            var key = GetRedisKey(code, _contextAccessor?.HttpContext?.ResolveClientIp());
            var dataModel = new RegistrationRazorDataModel
            {
                Title = CoreStrings.Template_Registration_Title.Format(model.Login),
                Code = code,
                Model = model
            };
            try
            {
                var html = await _razor.RenderToStringAsync(RazorService.Templates.Registration, dataModel);
                _email.Send(CoreSettings.SmtpConfig.Sender, model.Email, dataModel.Title, html, true);
                await _redis.Database.StringSetAsync(key, model.ToJson(), _redisKeyTtl);

                return true;
            }
            catch (Exception e)
            {
                e.Data[nameof(model)] = model;
                SentrySdk.CaptureException(e);

                return false;
            }
        }

        /// <summary>
        /// Второй этап - подтверждение регистрации
        /// </summary>
        /// <param name="code">Код из email</param>
        /// <returns></returns>
        public async Task<UserViewModel> ConfirmRegistration(string code)
        {
            var key = GetRedisKey(code, _contextAccessor?.HttpContext?.ResolveClientIp());
            var value = await _redis.GetAsync<string>(key);

            if (value == null || !value.TryCastTo<UserRegisterModel>(out var model))
            {
                return null;
            }

            var entity = _users.Add(model);

            if (entity != null)
            {
                await _redis.Database.KeyDeleteAsync(key);

                return _users.GetOne(entity.Id);
            }

            return null;
        }

        public void Dispose()
        {
            _users.Dispose();
            _email.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _users.DisposeAsync();
        }
    }
}
