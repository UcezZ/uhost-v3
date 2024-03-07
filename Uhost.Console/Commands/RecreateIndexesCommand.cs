using CommandLine;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Linq;
using System.Reflection;
using Uhost.Core;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using static System.Console;

namespace Uhost.Console.Commands
{
    [Verb("recreateix", HelpText = "Пересоздаёт все индексы полей сущностей во всех контекстах БД")]
    public sealed class RecreateIndexesCommand : BaseCommand
    {
        private const string _sqlGet = @"SELECT 
    pi.indexrelname,
    i.indexdef
FROM 
    pg_stat_user_indexes pi
    INNER JOIN pg_indexes i ON i.indexname = pi.indexrelname
WHERE
    pi.relname = ANY(@tables)
    AND pi.indexrelname NOT ILIKE 'PK_%'
ORDER BY 
    pi.relname ASC,
    pi.indexrelname ASC";
        private const string _ixDropFmt = "DROP INDEX \"{0}\"";

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
                .Select(e => Tools.GetEntityTableName(e))
                .ToList();

            foreach (var ctx in ctxs)
            {
                WriteLine(ctx.GetType().Name);

                using (ctx)
                {
                    var indexes = ctx.Database
                        .FromSqlRaw<string, string>(_sqlGet, new NpgsqlParameter("tables", tables))
                        .ToList();

                    var szTotalBefore = ctx.Database.GetSize();

                    using (var trx = ctx.Database.BeginTransaction())
                    {
                        try
                        {

                            foreach (var ix in indexes)
                            {
                                Write($"    {ix.Item1}: ");

                                var dropSql = _ixDropFmt.Format(ix.Item1);

                                using (var timer = new Timer())
                                {
                                    ctx.Database.ExecuteSqlRaw(dropSql);
                                    Write("DROP ");
                                    ctx.Database.ExecuteSqlRaw(ix.Item2);
                                    WriteLine($"OK in {timer.Milliseconds} ms");
                                }
                            }

                            trx.Commit();

                            var szTotalAfter = ctx.Database.GetSize();

                            WriteLine($"Total freed: {(szTotalBefore - szTotalAfter).ToHumanSize()}");
                        }
                        catch
                        {
                            trx.Rollback();
                            throw;
                        }
                    }
                }
            }
        }
    }
}
