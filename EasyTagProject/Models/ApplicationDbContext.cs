using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyTagProject.Models.Notifications;
//using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace EasyTagProject.Models
{
    // Class that allows the connection to EasyTagDB using EF
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.Start, a.RoomId})
                .IsUnique();

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.End, a.RoomId })
                .IsUnique();

            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.Date, n.RoomId})
                .IsUnique();
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
