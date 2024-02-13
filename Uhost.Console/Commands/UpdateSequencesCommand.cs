using CommandLine;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Uhost.Core;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using static System.Console;

namespace Uhost.Console.Commands
{
    [Verb("updateseq", HelpText = "Обновляет последовательности генерации ИД сущностей во всех контекстах БД")]
    public sealed class UpdateSequencesCommand : BaseCommand
    {
        private const string _sqlGet = @"SELECT relname 
FROM pg_class 
WHERE relkind = 'S'
ORDER BY relname";
        private const string _seqMaxFmt = @"SELECT (COALESCE(MAX(""{0}""), 0) + 1)::INT FROM ""{1}""";
        private const string _sqlSeqUpdateFmt = @"ALTER SEQUENCE ""{0}"" RESTART WITH {1}";
        private const string _sqlSeqValFmt = @"SELECT last_value FROM ""{0}""";

        protected override void Run()
        {
            var ctxTypes = Assembly
                 .GetAssembly(typeof(CoreSettings))
                 .GetTypes()
                 .Where(e => typeof(DbContext).IsAssignableFrom(e) && e.IsClass)
                 .ToList();

            var methodInfo = typeof(DependencyInjectionExtensions).GetMethods()
                .FirstOrDefault(e => e.Name == nameof(DependencyInjectionExtensions.GetDbContextScope) && e.IsGenericMethod);

            var ctxObjects = ctxTypes
                .Select(e => methodInfo.MakeGenericMethod(e).Invoke(null, Provider.AsSingleElementEnumerable().ToArray()))
                .ToList();

            var ctxs = ctxObjects
                .OfType<DbContext>()
                .ToList();

            var tables = Tools
                .EnumerateEntities()
                .Select(e => Tools.GetEntityTableName(e));

            var regex = new Regex($"^({tables.Join("|")})_(\\w*)_seq");

            foreach (var ctx in ctxs)
            {
                WriteLine(ctx.GetType().Name);

                using (ctx)
                {
                    var dbseq = ctx.Database
                        .FromSqlRaw<string>(_sqlGet)
                        .ToList();
                    var seqToProcess = dbseq
                        .Select(e => regex.Match(e))
                        .Where(e => e.Success && e.Groups.Count > 2);

                    foreach (var match in seqToProcess)
                    {
                        Write($"    {match.Groups[0].Value}: ");

                        var maxSql = _seqMaxFmt.Format(match.Groups[2].Value, match.Groups[1].Value);
                        ctx.Database.TryExecuteSqlRaw(maxSql, out int newValue);

                        var update = _sqlSeqUpdateFmt.Format(match.Groups[0].Value, newValue);
                        ctx.Database.ExecuteSqlRaw(update);

                        var get = _sqlSeqValFmt.Format(match.Groups[0].Value);

                        if (ctx.Database.TryExecuteSqlRaw(get, out object value))
                        {
                            WriteLine(value);
                        }
                        else
                        {
                            WriteLine("Failed to fetch");
                        }
                    }
                }
            }
        }
    }
}
