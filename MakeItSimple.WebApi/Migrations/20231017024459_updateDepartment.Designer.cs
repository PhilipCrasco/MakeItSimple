﻿// <auto-generated />
using System;
using MakeItSimple.WebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20231017024459_updateDepartment")]
    partial class updateDepartment
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("MakeItSimple.WebApi.Domain.Setup.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<int>("AddedBy")
                        .HasColumnType("int")
                        .HasColumnName("added_by");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("DepartmentName")
                        .HasColumnType("longtext")
                        .HasColumnName("department_name");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_active");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("longtext")
                        .HasColumnName("modified_by");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_departments");

                    b.HasIndex("AddedBy")
                        .IsUnique()
                        .HasDatabaseName("ix_departments_added_by");

                    b.ToTable("departments", (string)null);
                });

            modelBuilder.Entity("MakeItSimple.WebApi.Domain.Users.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<int?>("AddedBy")
                        .HasColumnType("int")
                        .HasColumnName("added_by");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int")
                        .HasColumnName("department_id");

                    b.Property<string>("Firstname")
                        .HasColumnType("longtext")
                        .HasColumnName("firstname");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_active");

                    b.Property<string>("Lastname")
                        .HasColumnType("longtext")
                        .HasColumnName("lastname");

                    b.Property<string>("Password")
                        .HasColumnType("longtext")
                        .HasColumnName("password");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<int?>("UserRoleId")
                        .HasColumnType("int")
                        .HasColumnName("user_role_id");

                    b.Property<string>("Username")
                        .HasColumnType("longtext")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("AddedBy")
                        .IsUnique()
                        .HasDatabaseName("ix_users_added_by");

                    b.HasIndex("DepartmentId")
                        .IsUnique()
                        .HasDatabaseName("ix_users_department_id");

                    b.HasIndex("UserRoleId")
                        .IsUnique()
                        .HasDatabaseName("ix_users_user_role_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("MakeItSimple.WebApi.Domain.Users.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<int>("AddedBy")
                        .HasColumnType("int")
                        .HasColumnName("added_by");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<bool>("IsActive")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_active");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("longtext")
                        .HasColumnName("modified_by");

                    b.Property<string>("Permissions")
                        .HasColumnType("longtext")
                        .HasColumnName("permissions");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("updated_at");

                    b.Property<string>("UserRoleName")
                        .HasColumnType("longtext")
                        .HasColumnName("user_role_name");

                    b.HasKey("Id")
                        .HasName("pk_user_role");

                    b.HasIndex("AddedBy")
                        .IsUnique()
                        .HasDatabaseName("ix_user_role_added_by");

                    b.ToTable("user_role", (string)null);
                });

            modelBuilder.Entity("MakeItSimple.WebApi.Domain.Setup.Department", b =>
                {
                    b.HasOne("MakeItSimple.WebApi.Domain.Users.User", "AddedByUser")
                        .WithOne()
                        .HasForeignKey("MakeItSimple.WebApi.Domain.Setup.Department", "AddedBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_departments_users_added_by");

                    b.Navigation("AddedByUser");
                });

            modelBuilder.Entity("MakeItSimple.WebApi.Domain.Users.User", b =>
                {
                    b.HasOne("MakeItSimple.WebApi.Domain.Users.User", "AddedByUser")
                        .WithOne()
                        .HasForeignKey("MakeItSimple.WebApi.Domain.Users.User", "AddedBy")
                        .HasConstraintName("fk_users_users_added_by_user_id");

                    b.HasOne("MakeItSimple.WebApi.Domain.Setup.Department", "Department")
                        .WithOne("User")
                        .HasForeignKey("MakeItSimple.WebApi.Domain.Users.User", "DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_users_departments_department_id");

                    b.HasOne("MakeItSimple.WebApi.Domain.Users.UserRole", "UserRole")
                        .WithOne("User")
                        .HasForeignKey("MakeItSimple.WebApi.Domain.Users.User", "UserRoleId")
                        .HasConstraintName("fk_users_user_role_user_role_id");

                    b.Navigation("AddedByUser");

                    b.Navigation("Department");

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("MakeItSimple.WebApi.Domain.Users.UserRole", b =>
                {
                    b.HasOne("MakeItSimple.WebApi.Domain.Users.User", "AddedByUser")
                        .WithOne()
                        .HasForeignKey("MakeItSimple.WebApi.Domain.Users.UserRole", "AddedBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_user_role_users_added_by");

                    b.Navigation("AddedByUser");
                });

            modelBuilder.Entity("MakeItSimple.WebApi.Domain.Setup.Department", b =>
                {
                    b.Navigation("User");
                });

            modelBuilder.Entity("MakeItSimple.WebApi.Domain.Users.UserRole", b =>
                {
                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
