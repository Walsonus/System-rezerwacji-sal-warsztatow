using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Core;

namespace TestReservationSystem
{
    public class TestRoomService
    {
        [Fact]
        public void GetAllRooms_ShouldReturnEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ReservationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (var context = new ReservationDbContext(options))
            {
                var RoomService = new RoomService(context);

                // Assert
                Assert.Empty(RoomService.GetAllRooms());
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public void GetAllRooms_ShouldReturnListOfRooms()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ReservationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (var context = new ReservationDbContext(options))
            {
                var RoomService = new RoomService(context);
                // Act
                RoomService.AddRoom(new Room() { Id = 1, Name = "Room 1", Capacity = 10 });
                RoomService.AddRoom(new Room() { Id = 2, Name = "Room 2", Capacity = 20 });

                // Assert
                Assert.Equal(2, RoomService.GetAllRooms().Count);
                context.Database.EnsureDeleted();
            }
        }
    }
}
