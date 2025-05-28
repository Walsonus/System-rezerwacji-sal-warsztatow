using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Core
{
    public class Room
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        //public List<Reservation> Reservations { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public Room(string name, int capacity)
        {
            this.Name = name;
            this.Capacity = capacity;
            Reservations = new List<Reservation>();
        }
    }
}
