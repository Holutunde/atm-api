using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces
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
