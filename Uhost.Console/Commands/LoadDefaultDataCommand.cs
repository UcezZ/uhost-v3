using CommandLine;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uhost.Console.Models;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Role;
using Uhost.Core.Repositories;
using static System.Console;
using static Uhost.Core.Data.Entities.Right;

namespace Uhost.Console.Commands
{
    [Verb("loaddefaultdata", HelpText = "Загрузка данных по умолчанию")]
    public sealed class LoadDefaultDataCommand : BaseCommand
    {
        private static readonly DefaultDataModel _defaultObject = new DefaultDataModel
        {
            Rights = new List<DefaultRightModel> { new DefaultRightModel { Name = string.Empty } },
            Roles = new List<DefaultRoleModel> { new DefaultRoleModel { Name = string.Empty, RightIds = new List<int> { 0 } } },
            Users = new List<DefaultUserModel> { new DefaultUserModel { Description = string.Empty, Name = string.Empty, Password = string.Empty, Login = string.Empty, RoleIds = new List<int> { 0 } } },
            SuperAdmin = new DefaultUserModel { Description = string.Empty, Name = string.Empty, Password = string.Empty, Login = string.Empty },
            SuperRole = new DefaultRoleModel { Name = string.Empty }
        };

        [Option("file", Required = true, HelpText = "Файл JSON с параметрами по умолчанию")]
        public string FileName { get; set; }

        protected override void Run()
        {
            var fileInfo = new FileInfo(FileName);

            if (!fileInfo.Exists)
            {
                WriteLine($"Файл \"{fileInfo.FullName}\" не найден");
                return;
            }
            if (!fileInfo.TryReadAsJson<DefaultDataModel>(out var data, out var exception))
            {
                WriteLine($"Ошибка чтения файла: {exception?.Message}\r\n{exception?.StackTrace}");
                WriteLine($"Содержимое должно иметь следующий формат:\r\n\r\n{_defaultObject.ToJson(Formatting.Indented)}");
                return;
            }

            using (var dbContext = Provider.GetDbContextScope<PostgreSqlDbContext>())
            {
                var rightRepo = new RightRepository(dbContext);
                var roleRepo = new RoleRepository(dbContext);
                var userRepo = new UserRepository(dbContext);

                if (data.Rights != null)
                {
                    var enumRights = Enum.GetValues<Rights>().Select(e => (int)e);
                    var defaultRights = data.Rights.Select(e => e.Id);

                    var miggingInYml = enumRights.Except(defaultRights).ToList();
                    var missingInEnum = defaultRights.Except(enumRights).ToList();

                    if (missingInEnum.Any())
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine($"В перечислении '{typeof(Rights).FullName}' отсутствуют значения, присутствующие в файле '{FileName}':\r\n {string.Join("", missingInEnum.Select(e => $"\r\n    {e}: {data.Rights.Where(a => a.Id == e).Select(a => a.Name).FirstOrDefault()}"))}");
                        ResetColor();
                        WriteLine();
                    }

                    if (miggingInYml.Any())
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine($"В файле '{FileName}' отсутствуют значения, присутствующие в перечислении '{typeof(Rights).FullName}':\r\n {string.Join("", miggingInYml.Select(e => $"\r\n    {e}: {(Rights)e}"))}");
                        ResetColor();
                        WriteLine();
                    }

                    if (missingInEnum.Any() || miggingInYml.Any())
                    {
                        throw new Exception($"File '{FileName}' content mismatches enum '{typeof(Rights).FullName}' or vice versa");
                    }

                    WriteLine("Загрузка прав");

                    foreach (var model in data.Rights)
                    {
                        Write($"    #{model.Id}\t[{model.Name}]");
                        var affected = rightRepo.StrictAddOrUpdate(model, nameof(model.Name));
                        WriteLine($": {affected}");
                    }

                    var deleted = rightRepo.HardDeleteAll(e => !data.Rights.Select(e => e.Id).Contains(e.Id));
                    WriteLine($"Удалено прав: {deleted}");
                    WriteLine();
                }

                if (data.Roles != null && data.Roles.Any())
                {
                    WriteLine("Загрузка ролей");

                    foreach (var model in data.Roles)
                    {
                        Write($"    #{model.Id}\t[{model.Name}]");
                        var affected = roleRepo.StrictAddOrUpdate(model, dbset => dbset.Include(e => e.RoleRights), nameof(model.Name));
                        WriteLine($": {affected}");
                    }

                    WriteLine();
                }

                if (data.Users != null && data.Users.Any())
                {
                    WriteLine("Загрузка пользователей");

                    foreach (var model in data.Users)
                    {
                        Write($"    #{model.Id}\t[{model.Name}]");
                        var affected = userRepo.StrictAddOrUpdate(model, dbset => dbset.Include(e => e.UserRoles), nameof(model.Login));
                        WriteLine($": {affected}");
                    }

                    WriteLine();
                }

                if (data.SuperRole != null)
                {
                    Write("Загрузка суперроли");
                    data.SuperRole.RightIds = rightRepo.PrepareQuery().Select(e => e.Id).ToList();

                    var affected = roleRepo.StrictAddOrUpdate(data.SuperRole, dbset => dbset.Include(e => e.RoleRights), nameof(data.SuperRole.Name));
                    WriteLine($": {affected}");
                }

                if (data.SuperAdmin != null)
                {
                    Write("Загрузка суперадмина");

                    if (data.SuperRole == null)
                    {
                        data.SuperAdmin.RoleIds = roleRepo.PrepareQuery(new RoleQueryModel { }).Select(e => e.Id).ToList();
                    }
                    else
                    {
                        data.SuperAdmin.RoleIds = new List<int> { data.SuperRole.Id };

                        var affected = userRepo.StrictAddOrUpdate(data.SuperAdmin, dbset => dbset.Include(e => e.UserRoles), nameof(data.SuperRole.Name));
                        WriteLine($": {affected}");
                    }
                }
            }
        }
    }
}
