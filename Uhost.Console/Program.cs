using CommandLine;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Uhost.Console.Commands;
using Uhost.Core;
using Uhost.Core.Data;
using static System.Console;

namespace Uhost.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddHangfireServer();

            services.AddDbContext<PostgreSqlDbContext>(e => e.UseNpgsql(CoreSettings.SqlConnectionString));
            services.AddDbContext<PostgreSqlLogDbContext>(e => e.UseNpgsql(CoreSettings.SqlLogConnectionString));

            var provider = services.BuildServiceProvider();

            WriteLine();
            CancelKeyPress += (s, e) => Process.GetCurrentProcess().Close();

            // определяем команды по релизации ICommand
            var commands = Assembly
                .GetAssembly(typeof(BaseCommand))
                .GetTypes()
                .Where(e => typeof(BaseCommand).IsAssignableFrom(e) && e.IsClass)
                .ToArray();

            Parser.Default
                .ParseArguments(args, commands)
                .WithParsed<BaseCommand>(c => c.UseServiceProvider(provider).Run());
        }
    }
}
