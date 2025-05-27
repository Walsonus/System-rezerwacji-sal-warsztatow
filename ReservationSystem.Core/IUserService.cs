using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Core
{
    public interface IUserService
    {
        User Register(string username, string password, UserRole role);
        User Authenticate(string username, string password);
        User GetUserById(int userId);
        bool HasAccess(User user, string requiredRole);
    }
}
