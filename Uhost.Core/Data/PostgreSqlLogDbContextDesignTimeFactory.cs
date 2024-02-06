using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Uhost.Core.Data
{
    /// <summary>
    /// Фабрика контекстов. Используется dotnet-ef для наката миграций https://learn.microsoft.com/ru-ru/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
    /// </summary>
    public class PostgreSqlLogDbContextDesignTimeFactory : IDesignTimeDbContextFactory<PostgreSqlLogDbContext>
    {
        public PostgreSqlLogDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PostgreSqlLogDbContext>()
                .UseNpgsql(CoreSettings.SqlLogConnectionString, b => b.MigrationsAssembly(typeof(PostgreSqlLogDbContext).Assembly.GetName().Name).CommandTimeout(1800));

            return new PostgreSqlLogDbContext(builder.Options);
        }
    }
}
