using CommandLine;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using Uhost.Core;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using static System.Console;

namespace Uhost.Console.Commands
{
    [Verb("dbvacuum", HelpText = "Выполняет команду VACUUM во всех контекстах БД")]
    public sealed class DbVacuumCommand : BaseCommand
    {
        protected override void Run()
        {
            var ctxTypes = Assembly
                 .GetAssembly(typeof(CoreSettings))
                 .GetTypes()
                 .Where(e => typeof(DbContext).IsAssignableFrom(e) && e.IsClass)
                 .ToList();

            var ctxObjects = ctxTypes.Select(e => e.Instantiate()).ToList();

            var ctxs = ctxObjects.OfType<DbContext>().ToList();

            foreach (var ctx in ctxs)
            {
                using (ctx)
                using (var timer = new Timer())
                {
                    var before = ctx.Database.GetSize();
                    ctx.Database.ExecuteSqlRaw("VACUUM");
                    var after = ctx.Database.GetSize();
                    WriteLine($"{ctx.GetType().Name}: {(before - after).ToHumanSize()} освобождено за {timer.Milliseconds} мс");
                }
            }
        }
    }
}
