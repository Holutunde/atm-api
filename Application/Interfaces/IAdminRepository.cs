
using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces
{
   public interface IAdminRepository
    {
        Task<Admin> Register(Admin admin);
        Task<Admin> Login(OnlineLoginDto loginDto);
        Task<Admin> GetAdminById(int id);
        Task<Admin> GetAdminByEmail(string email);
        Task<ICollection<Admin>> GetAllAdmins();
        Task UpdateAdminDetails(int id, AdminDto adminDto);

        Task UpdateAdminBalance(int id, double newBalance);
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmail(string email);
        Task<ICollection<User>> GetAllUsers();
        Task<Admin> GetAdminByAccountNumber(long accountNumber);
        Task DeleteAdminAccount(int id);
        Task DeleteUserAccount(string email);
    }
}
