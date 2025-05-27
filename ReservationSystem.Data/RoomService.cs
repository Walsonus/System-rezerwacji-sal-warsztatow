using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Core;

namespace ReservationSystem.Data
{
    public class RoomService : IRoomService
    {
        // getting the context from the database
        private ReservationDbContext context;
        private IUserService userService;
        public RoomService(ReservationDbContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        public RoomService(ReservationDbContext context) : this(context, null)
        {
        }

        //method to add a reservation
        public void AddReservation(Reservation reservation, User user)
        {

            if (userService == null || !userService.HasAccess(user, "Prowadzacy")) throw new UnauthorizedAccessException("Tylko prowadzący mogą dokonywać rezerwacji");
            // Walidacja
            if (reservation.Room == null && reservation.RoomId == 0)
            {
                throw new ArgumentException("Rezerwacja musi mieć przypisany pokój");
            }

            // Jeśli przekazano obiekt Room, ale nie RoomId
            if (reservation.Room != null && reservation.RoomId == 0)
            {
                reservation.RoomId = reservation.Room.Id;
            }

            // Sprawdź czy pokój istnieje
            var roomExists = context.Rooms.Any(r => r.Id == reservation.RoomId);
            if (!roomExists)
            {
                throw new ArgumentException($"Pokój o ID {reservation.RoomId} nie istnieje");
            }
            reservation.UserId = user.UserId;
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
        public Room GetRoomById(int id)
        {
            return context.Rooms.FirstOrDefault(r => r.Id == id);
        }

        //returns a list of available rooms
        public List<Room> GetAvaiableRooms(DateTime start, DateTime end)
            => context.Rooms
                .Where(room => !room.Reservations.Any(reservation =>
                                  reservation.StartDate < end && reservation.EndDate > start)).ToList();


        public List<Reservation> GetUserReservations(int userId)
        {
            return context.Reservations
                .Include(r => r.Room)
                .Where(r => r.UserId == userId)
                .AsNoTracking()
                .ToList();
        }

    }
}
