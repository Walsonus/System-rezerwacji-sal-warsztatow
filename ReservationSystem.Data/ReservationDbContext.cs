using Microsoft.EntityFrameworkCore;
using ReservationSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Data
{
    public class ReservationDbContext : DbContext
    {
        DbSet<Room> Rooms { get; set; }
        DbSet<Reservation> Reservations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ReservationSystem;Trusted_Connection=True;");
        }
    }
}
