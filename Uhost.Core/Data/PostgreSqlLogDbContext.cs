using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using Uhost.Core.Common;
using Uhost.Core.Data.Entities;

namespace Uhost.Core.Data
{
    public class PostgreSqlLogDbContext : DbContext
    {
        private const string _sqlNow = "now()";
        private const string _sqlEmptyJson = "'{}'::JSONB";

        public DbSet<Log> Logs { get; private set; }

        public PostgreSqlLogDbContext(DbContextOptions<PostgreSqlLogDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var assembly = Assembly.GetAssembly(typeof(PostgreSqlDbContext));

            optionsBuilder.UseNpgsql(CoreSettings.SqlLogConnectionString, e => e
                .MigrationsAssembly(assembly.FullName)
                .CommandTimeout(CoreSettings.SqlCommandTimeoutSeconds));

            if (LocalEnvironment.IsDev)
            {
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            builder.Entity<Log>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql(_sqlNow);

            builder.Entity<Log>()
                .Property(e => e.Data)
                .HasDefaultValueSql(_sqlEmptyJson);
        }

        public void DetachAllEntities()
        {
            ChangeTracker.Entries().ToList()
                .ForEach(x => x.State = EntityState.Detached);
        }
    }
}
