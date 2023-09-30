﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Uhost.Core.Data;

namespace Uhost.Core.Data.Migrations
{
    [DbContext(typeof(PostgreSqlDbContext))]
    partial class PostgreSqlDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Uhost.Core.Data.Entities.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("now()");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("Digest")
                        .IsRequired()
                        .HasColumnType("char(40)");

                    b.Property<int?>("DynId")
                        .HasColumnType("integer");

                    b.Property<string>("DynName")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(16)")
                        .HasDefaultValue("");

                    b.Property<string>("Mime")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("application/octet-stream");

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("file.bin");

                    b.Property<int>("Size")
                        .HasColumnType("integer");

                    b.Property<string>("Token")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(32)")
                        .HasDefaultValueSql("gen_file_token()");

                    b.Property<string>("Type")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(16)")
                        .HasDefaultValue("");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Token")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.Right", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Rights");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("now()");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.RoleRight", b =>
                {
                    b.Property<int>("RightId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("RightId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleRights");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("now()");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("Desctiption")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("");

                    b.Property<string>("Email")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("");

                    b.Property<DateTime?>("LastVisitAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("Login")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("");

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("");

                    b.Property<string>("Password")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("");

                    b.Property<string>("Theme")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(8)")
                        .HasDefaultValue("");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.Video", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp")
                        .HasDefaultValueSql("now()");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp");

                    b.Property<string>("Description")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval");

                    b.Property<string>("Name")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("");

                    b.Property<string>("Token")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(16)")
                        .HasDefaultValueSql("gen_video_token()");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Token")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.File", b =>
                {
                    b.HasOne("Uhost.Core.Data.Entities.User", "User")
                        .WithMany("Files")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.RoleRight", b =>
                {
                    b.HasOne("Uhost.Core.Data.Entities.Right", "Right")
                        .WithMany("RoleRights")
                        .HasForeignKey("RightId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Uhost.Core.Data.Entities.Role", "Role")
                        .WithMany("RoleRights")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Right");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.UserRole", b =>
                {
                    b.HasOne("Uhost.Core.Data.Entities.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Uhost.Core.Data.Entities.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.Video", b =>
                {
                    b.HasOne("Uhost.Core.Data.Entities.User", "User")
                        .WithMany("Videos")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.Right", b =>
                {
                    b.Navigation("RoleRights");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.Role", b =>
                {
                    b.Navigation("RoleRights");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Uhost.Core.Data.Entities.User", b =>
                {
                    b.Navigation("Files");

                    b.Navigation("UserRoles");

                    b.Navigation("Videos");
                });
#pragma warning restore 612, 618
        }
    }
}
