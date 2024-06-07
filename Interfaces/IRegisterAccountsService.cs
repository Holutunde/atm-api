using ATMAPI.Models;

namespace ATMAPI.Services
{
    public interface IRegisteredUsersService
    {
        void AddUser(User User); 
        void AddAdmin(Admin User); 
        ICollection<User> GetUsers();
        User GetUserByNumber(long AccountNumber);
        void UpdateUser(User  updatedUser);
        void DeleteUser(long AccountNumber);
    }
}