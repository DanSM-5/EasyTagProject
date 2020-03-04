﻿// <auto-generated />
using System;
using EasyTagProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EasyTagProject.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EasyTagProject.Models.Appointment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Course")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime2");

                    b.Property<int>("RoomId")
                        .HasColumnType("int");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("EasyTagProject.Models.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Block")
                        .IsRequired()
                        .HasColumnType("nvarchar(1)");

                    b.Property<int>("Floor")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<string>("RoomCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("EasyTagProject.Models.Schedule", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("EasyTagProject.Models.Appointment", b =>
                {
                    b.HasOne("EasyTagProject.Models.Schedule", null)
                        .WithMany("Appointments")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EasyTagProject.Models.Schedule", b =>
                {
                    b.HasOne("EasyTagProject.Models.Room", "Room")
                        .WithOne("Schedule")
                        .HasForeignKey("EasyTagProject.Models.Schedule", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
