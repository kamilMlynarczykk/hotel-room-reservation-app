using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelReservationApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelReservationApp.Data
{
    public class HotelReservationAppContext : IdentityDbContext<AppUser>
    {
        public HotelReservationAppContext (DbContextOptions<HotelReservationAppContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Room { get; set; }
        public DbSet<RoomContents> RoomContents { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationHistory> ReservationHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.AppUser)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.AppUserID);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Room)
                .WithMany(rm => rm.Reservations)
                .HasForeignKey(r => r.RoomID);
        }

    }
}
