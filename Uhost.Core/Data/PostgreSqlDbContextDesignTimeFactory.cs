using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Uhost.Core.Data
{
    /// <summary>
    /// Фабрика контекстов. Используется dotnet-ef для наката миграций https://learn.microsoft.com/ru-ru/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
    /// </summary>
    public class PostgreSqlDbContextDesignTimeFactory : IDesignTimeDbContextFactory<PostgreSqlDbContext>
    {
        public PostgreSqlDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PostgreSqlDbContext>()
                .UseNpgsql(CoreSettings.SqlConnectionString, b => b.MigrationsAssembly(typeof(PostgreSqlDbContext).Assembly.GetName().Name).CommandTimeout(1800));

            return new PostgreSqlDbContext(builder.Options);
        }
    }
}
