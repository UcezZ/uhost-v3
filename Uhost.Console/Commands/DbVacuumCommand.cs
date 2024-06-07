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
                 .Where(e => e.IsAssignableTo<DbContext>() && e.IsClass)
                 .ToList();

            var methodInfo = typeof(DependencyInjectionExtensions).GetMethods()
                .FirstOrDefault(e => e.Name == nameof(DependencyInjectionExtensions.GetDbContextInstance) && e.IsGenericMethod);

            var ctxObjects = ctxTypes
                .Select(e => methodInfo.MakeGenericMethod(e).Invoke(null, Provider.AsSingleElementEnumerable().ToArray()))
                .ToList();

            var ctxs = ctxObjects.OfType<DbContext>().ToList();

            foreach (var ctx in ctxs)
            {
                using (ctx)
                using (var timer = new Timer())
                {
                    var before = ctx.Database.GetSize();
                    ctx.Database.ExecuteSqlRaw("VACUUM FULL");
                    var after = ctx.Database.GetSize();
                    WriteLine($"{ctx.GetType().Name}: {(before - after).ToHumanSize()} освобождено за {timer.Milliseconds} мс");
                }
            }
        }
    }
}
