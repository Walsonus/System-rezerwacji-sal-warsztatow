using ReservationSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Data
{
    public class UserService : IUserService
    {
        private readonly ReservationDbContext context;
        public UserService(ReservationDbContext context) => this.context = context;

        public User Register(string username, string password, UserRole role)
        {
            var user = new User(username, HashPassword(password), role);
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }

        public User Authenticate(string username, string password)
        {
            var hashedPassword = HashPassword(password);
            return context.Users.FirstOrDefault(user => user.Login == username && user.Password == hashedPassword);
        }

        public User GetUserById(int id)
        {
            return context.Users.FirstOrDefault(user => user.UserId == id);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool HasAccess(User user, string requiredRole)
        {
            if (user == null) return false;
            return user.Role.ToString() == requiredRole || user.Role == UserRole.Admin;
        }
    }
}
