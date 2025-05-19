using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using ReservationSystem.Data;
using ReservationSystem.Core;
using System.ComponentModel.DataAnnotations;

namespace TestReservationSystem
{
    public class TestUserService
    {
        [Fact]
        public void Authenticate_ShouldReturnUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ReservationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (var context = new ReservationDbContext(options))
            {
                var UserService = new UserService(context);

                var user = UserService.Register("Jan", "Kowalski", UserRole.Student);
                var user2 = UserService.Authenticate("Jan", "Kowalski");

                // Assert
                Assert.Equal(user, user2);
                context.Database.EnsureDeleted();
            }
        }
        [Fact]
        public void FindUserById_ShouldReturnUser()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ReservationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using (var context = new ReservationDbContext(options))
            {
                var UserService = new UserService(context);

                var user = UserService.Register("Jan", "Janek123!", UserRole.Prowadzacy);
                var user2 = UserService.Register("Anna", "Ania321!!", UserRole.Student);
                Assert.Equal(UserService.GetUserById(1), user);
                Assert.Equal(UserService.GetUserById(2), user2);
            }
        }
    }
}
