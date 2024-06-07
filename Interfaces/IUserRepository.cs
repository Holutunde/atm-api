using ATMAPI.Dto;
using ATMAPI.Models;

namespace ATMAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Register(User user);
        Task<User> Login(OnlineLoginDto loginDto);
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByAccountNumber(long accountNumber);
        Task UpdateUserDetails(int id, UserDto userDto);
        Task UpdateUserBalance(int id, double newBalance);
    }
}
