﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20210103163921_NewSettings")]
    partial class NewSettings
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("Domain.Boat", b =>
                {
                    b.Property<int>("BoatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("BoatCount")
                        .HasColumnType("int");

                    b.Property<int>("BoatLength")
                        .HasColumnType("int");

                    b.Property<string>("BoatName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SettingId")
                        .HasColumnType("int");

                    b.HasKey("BoatId");

                    b.HasIndex("SettingId");

                    b.ToTable("Boats");
                });

            modelBuilder.Entity("Domain.GameSave", b =>
                {
                    b.Property<int>("GameSaveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("BoardSize_")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MoveLog_")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("Player1")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Player2")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ShipList1_")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("ShipList2_")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("ShipsReq_")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<int>("Touch_")
                        .HasColumnType("int");

                    b.HasKey("GameSaveId");

                    b.ToTable("GameSaves");
                });

            modelBuilder.Entity("Domain.Setting", b =>
                {
                    b.Property<int>("SettingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("BoardSize")
                        .HasColumnType("int");

                    b.Property<int>("PlaceType")
                        .HasColumnType("int");

                    b.Property<string>("Player_1")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Player_2")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Touch")
                        .HasColumnType("int");

                    b.HasKey("SettingId");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Domain.Boat", b =>
                {
                    b.HasOne("Domain.Setting", null)
                        .WithMany("Boats")
                        .HasForeignKey("SettingId");
                });

            modelBuilder.Entity("Domain.Setting", b =>
                {
                    b.Navigation("Boats");
                });
#pragma warning restore 612, 618
        }
    }
}