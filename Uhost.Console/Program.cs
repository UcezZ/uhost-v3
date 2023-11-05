using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Uhost.Console.Commands;
using Uhost.Core;
using Uhost.Core.Extensions;
using static System.Console;

namespace Uhost.Console
{
    static class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddUhostCoreServices();
            services.AddLogWriter();
            services.AddRabbitMqClient(CoreSettings.RabbitMqClientOptions);
            var provider = services.BuildServiceProvider();

            WriteLine();
            CancelKeyPress += (s, e) => Process.GetCurrentProcess().Close();

            // определяем команды по релизации BaseCommand
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
