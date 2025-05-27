using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Core
{
    public interface IRoomService
    {
        List<Room> GetAvaiableRooms(DateTime startDate, DateTime endDate);
        void AddReservation(Reservation reservation, User user);
        void AddRoom(Room room);
        List<Room> GetAllRooms();
        Room GetRoomById(int id);
        List<Reservation> GetUserReservations(int userId);
    }
}
