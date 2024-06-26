﻿// <auto-generated />
using System;
using HotelReservationApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HotelReservationApp.Migrations
{
    [DbContext(typeof(HotelReservationAppContext))]
    [Migration("20240529111559_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AppUser");
                });

            modelBuilder.Entity("HotelReservationApp.Models.RoomContents", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Balconies")
                        .HasColumnType("int");

                    b.Property<int>("Beds")
                        .HasColumnType("int");

                    b.Property<int>("Chairs")
                        .HasColumnType("int");

                    b.Property<int>("Desks")
                        .HasColumnType("int");

                    b.Property<int>("TV")
                        .HasColumnType("int");

                    b.Property<int>("Fridge")
                        .HasColumnType("int");

                    b.Property<int>("Kettle")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("RoomContents");
                });

            modelBuilder.Entity("Reservation", b =>
                {
                    b.Property<int>("ReservationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReservationID"));

                    b.Property<int>("AppUserID")
                        .HasColumnType("int");

                    b.Property<string>("CustomerID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("RoomID")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ReservationID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("RoomID");

                    b.ToTable("Reservation");
                });

            modelBuilder.Entity("Room", b =>
                {
                    b.Property<int>("RoomID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoomID"));

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<decimal>("PricePerNight")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("RoomNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoomType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoomContentsId")
                        .HasColumnType("int");

                    b.HasKey("RoomID");

                    b.HasIndex("RoomContentsId");

                    b.ToTable("Room");
                });

            modelBuilder.Entity("Reservation", b =>
                {
                    b.HasOne("AppUser", "appUser")
                        .WithMany("Reservations")
                        .HasForeignKey("CustomerID");

                    b.HasOne("Room", "Room")
                        .WithMany("Reservations")
                        .HasForeignKey("RoomID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");

                    b.Navigation("appUser");
                });

            modelBuilder.Entity("Room", b =>
                {
                    b.HasOne("HotelReservationApp.Models.RoomContents", "RoomContents")
                        .WithMany()
                        .HasForeignKey("RoomContentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RoomContents");
                });

            modelBuilder.Entity("AppUser", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("Room", b =>
                {
                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}