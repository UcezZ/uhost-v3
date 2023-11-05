using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using Uhost.Core.Common;
using Uhost.Core.Data.Entities;

namespace Uhost.Core.Data
{
    public class PostgreSqlDbContext : DbContext
    {
        private const string _sqlNow = "now()";
        private const string _sqlEmptyJsonArray = "[]";
        private const string _sqlEmptyJson = "{}";

        public DbSet<File> Files { get; private set; }
        public DbSet<Right> Rights { get; private set; }
        public DbSet<Role> Roles { get; private set; }
        public DbSet<RoleRight> RoleRights { get; private set; }
        public DbSet<User> Users { get; private set; }
        public DbSet<UserRole> UserRoles { get; private set; }
        public DbSet<Video> Videos { get; private set; }
        public DbSet<VideoReaction> VideoReactions { get; private set; }
        public DbSet<Comment> Comments { get; private set; }
        public DbSet<Playlist> Playlists { get; private set; }
        public DbSet<PlaylistEntry> PlaylistEntries { get; private set; }

        public PostgreSqlDbContext() : base() { }

        public PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var assembly = Assembly.GetAssembly(typeof(PostgreSqlDbContext));

            optionsBuilder.UseNpgsql(CoreSettings.SqlConnectionString, e => e
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

            // пользователь
            builder.Entity<User>()
                .HasMany(e => e.Roles)
                .WithMany(e => e.Users)
                .UsingEntity<UserRole>(
                    rel => rel
                        .HasOne(e => e.Role)
                        .WithMany(e => e.UserRoles),
                    rel => rel
                        .HasOne(e => e.User)
                        .WithMany(e => e.UserRoles),
                    rel => rel.HasKey(e => new
                    {
                        e.UserId,
                        e.RoleId
                    }));

            builder.Entity<User>()
                .HasOne(e => e.BlockedByUser)
                .WithMany(e => e.BlockedUsers)
                .HasForeignKey(e => e.BlockedByUserId)
                .HasPrincipalKey(e => e.Id);

            // роль
            builder.Entity<Role>()
                .HasMany(e => e.Rights)
                .WithMany(e => e.Roles)
                .UsingEntity<RoleRight>(
                    rel => rel
                        .HasOne(e => e.Right)
                        .WithMany(e => e.RoleRights),
                    rel => rel
                        .HasOne(e => e.Role)
                        .WithMany(e => e.RoleRights),
                    rel => rel.HasKey(e => new
                    {
                        e.RightId,
                        e.RoleId
                    }));

            // видео
            builder.Entity<Video>()
                .HasOne(e => e.User)
                .WithMany(e => e.Videos)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(e => e.Id);

            // файлы
            builder.Entity<File>()
                .HasOne(e => e.User)
                .WithMany(e => e.Files)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(e => e.Id);

            // комментарии
            builder.Entity<Comment>()
                .HasOne(e => e.User)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(e => e.Id);

            builder.Entity<Comment>()
                .HasOne(e => e.Video)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.VideoId)
                .HasPrincipalKey(e => e.Id);

            // реакции
            builder.Entity<VideoReaction>()
                .HasOne(e => e.User)
                .WithMany(e => e.VideoReactions)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(e => e.Id);

            builder.Entity<VideoReaction>()
                .HasOne(e => e.Video)
                .WithMany(e => e.VideoReactions)
                .HasForeignKey(e => e.VideoId)
                .HasPrincipalKey(e => e.Id);

            // плейлисты
            builder.Entity<Playlist>()
                .HasOne(e => e.User)
                .WithMany(e => e.Playlists)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(e => e.Id);

            // элементы плейлиста
            builder.Entity<PlaylistEntry>()
                .HasOne(e => e.Video)
                .WithMany(e => e.PlaylistEntries)
                .HasForeignKey(e => e.VideoId)
                .HasPrincipalKey(e => e.Id);

            builder.Entity<PlaylistEntry>()
                .HasOne(e => e.Playlist)
                .WithMany(e => e.PlaylistEntries)
                .HasForeignKey(e => e.PlaylistId)
                .HasPrincipalKey(e => e.Id);

            #region AddIsUniqueConstraint
            builder.Entity<Right>()
                .HasIndex(e => e.Name)
                .IsUnique();

            builder.Entity<Video>()
                .HasIndex(e => e.Token)
                .IsUnique();

            builder.Entity<File>()
                .HasIndex(e => e.Token)
                .IsUnique();
            #endregion

            #region AddFieldDefaultValue
            // права
            builder.Entity<Right>()
                .Property(e => e.Name)
                .HasDefaultValue(string.Empty);

            // роли
            builder.Entity<Role>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql(_sqlNow);

            builder.Entity<Role>()
                .Property(e => e.Name)
                .HasDefaultValue(string.Empty);

            // пользователи
            builder.Entity<User>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql(_sqlNow);

            builder.Entity<User>()
                .Property(e => e.Name)
                .HasDefaultValue(string.Empty);

            builder.Entity<User>()
                .Property(e => e.Desctiption)
                .HasDefaultValue(string.Empty);

            builder.Entity<User>()
                .Property(e => e.Login)
                .HasDefaultValue(string.Empty);

            builder.Entity<User>()
                .Property(e => e.Email)
                .HasDefaultValue(string.Empty);

            builder.Entity<User>()
                .Property(e => e.Password)
                .HasDefaultValue(string.Empty);

            builder.Entity<User>()
                .Property(e => e.Theme)
                .HasDefaultValue(string.Empty);

            // видео
            builder.Entity<Video>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql(_sqlNow);

            builder.Entity<Video>()
                .Property(e => e.Name)
                .HasDefaultValue(string.Empty);

            builder.Entity<Video>()
                .Property(e => e.Description)
                .HasDefaultValue(string.Empty);

            builder.Entity<Video>()
                .Property(e => e.Token)
                .HasDefaultValueSql("gen_video_token()");

            builder.Entity<Video>()
                .Property(e => e.AllowComments)
                .HasDefaultValue(true);

            builder.Entity<Video>()
                .Property(e => e.AllowReactions)
                .HasDefaultValue(true);

            // файл
            builder.Entity<File>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql(_sqlNow);

            builder.Entity<File>()
                .Property(e => e.Token)
                .HasDefaultValueSql("gen_file_token()");

            builder.Entity<File>()
                .Property(e => e.Mime)
                .HasDefaultValue("application/octet-stream");

            builder.Entity<File>()
                .Property(e => e.Name)
                .HasDefaultValue("file.bin");

            builder.Entity<File>()
                .Property(e => e.Type)
                .HasDefaultValue(string.Empty);

            builder.Entity<File>()
                .Property(e => e.DynName)
                .HasDefaultValue(string.Empty);

            // комментарии
            builder.Entity<Comment>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql(_sqlNow);

            builder.Entity<Comment>()
                .Property(e => e.Text)
                .HasDefaultValue(string.Empty);

            // реакции
            builder.Entity<VideoReaction>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql(_sqlNow);

            // плейлисты
            builder.Entity<Playlist>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql(_sqlNow);

            builder.Entity<Playlist>()
                .Property(e => e.Name)
                .HasDefaultValue(string.Empty);
            #endregion
        }

        public void DetachAllEntities()
        {
            ChangeTracker.Entries().ToList()
                .ForEach(x => x.State = EntityState.Detached);
        }
    }
}
