﻿// <auto-generated />
using System;
using Jobseeker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Jobseeker.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250215225618_AddRolesToUsers")]
    partial class AddRolesToUsers
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Jobseeker.Domain.Entities.JobApplication", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("AppliedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("JobId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("JobSeekerId")
                        .HasColumnType("uuid");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("JobApplications");
                });

            modelBuilder.Entity("Jobseeker.Domain.Entities.JobPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CompanyName")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<Guid?>("EmployerId")
                        .HasColumnType("uuid");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<DateTime>("PostedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("EmployerId");

                    b.ToTable("JobPosts");
                });

            modelBuilder.Entity("Jobseeker.Domain.Entities.JobSeekerDocument", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DocumentUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("JobApplicationId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("JobSeekerId")
                        .HasColumnType("uuid");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("JobApplicationId");

                    b.HasIndex("UserId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("Jobseeker.Domain.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Jobseeker.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.Property<Guid>("RolesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("uuid");

                    b.HasKey("RolesId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("RoleUser");
                });

            modelBuilder.Entity("Jobseeker.Domain.Entities.JobApplication", b =>
                {
                    b.HasOne("Jobseeker.Domain.Entities.User", null)
                        .WithMany("JobApplications")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Jobseeker.Domain.Entities.JobPost", b =>
                {
                    b.HasOne("Jobseeker.Domain.Entities.User", "Employer")
                        .WithMany()
                        .HasForeignKey("EmployerId");

                    b.Navigation("Employer");
                });

            modelBuilder.Entity("Jobseeker.Domain.Entities.JobSeekerDocument", b =>
                {
                    b.HasOne("Jobseeker.Domain.Entities.JobApplication", null)
                        .WithMany("Documents")
                        .HasForeignKey("JobApplicationId");

                    b.HasOne("Jobseeker.Domain.Entities.User", null)
                        .WithMany("Documents")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.HasOne("Jobseeker.Domain.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jobseeker.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Jobseeker.Domain.Entities.JobApplication", b =>
                {
                    b.Navigation("Documents");
                });

            modelBuilder.Entity("Jobseeker.Domain.Entities.User", b =>
                {
                    b.Navigation("Documents");

                    b.Navigation("JobApplications");
                });
#pragma warning restore 612, 618
        }
    }
}
