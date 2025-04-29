using ReservationSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Data
{
    public class RoomService : IRoomService
    {
        // getting the context from the database
        private ReservationDbContext context;
        public RoomService(ReservationDbContext context)
        {
            this.context = context;
        }

        //method to add a reservation
        public void AddReservation(Reservation reservation)
        {
            context.Reservations.Add(reservation);
            context.SaveChanges();
        }

        public void AddRoom(Room room)
        {
            context.Rooms.Add(room);
            context.SaveChanges();
        }

        public List<Room> GetAllRooms()
        {
            return context.Rooms.ToList();
        }

        //returns a list of available rooms
        public List<Room> GetAvaiableRooms(DateTime start, DateTime end)
            => context.Rooms
                .Where(room => !room.Reservations.Any(reservation =>
                                  reservation.StartDate < end && reservation.EndDate > start)).ToList();
    }
}
