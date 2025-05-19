using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Core
{

    public enum UserRole
    {
        Student,
        Prowadzacy,
        Admin
    }
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>(); //Kolekcja rezerwacji

        public User(string username, string password, UserRole role)
        {
            Login = username;
            Password = password;
            Role = role;
        }

        public User() { }
    }
}
