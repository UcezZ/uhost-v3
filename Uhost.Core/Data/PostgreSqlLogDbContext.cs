using Microsoft.EntityFrameworkCore;
using System.Linq;
using Uhost.Core.Data.Entities;

namespace Uhost.Core.Data
{
    public class PostgreSqlLogDbContext : DbContext
    {
        private const string _sqlNow = "now()";
        private const string _sqlEmptyJson = "'{}'::JSONB";

        public DbSet<Log> Logs { get; private set; }

        public PostgreSqlLogDbContext(DbContextOptions<PostgreSqlLogDbContext> options) : base(options) { }

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
