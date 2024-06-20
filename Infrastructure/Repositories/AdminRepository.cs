using Infrastructure.Data;
using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _context;

        public AdminRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Admin> Register(Admin admin)
        {
            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<Admin> Login(OnlineLoginDto loginDto)
        {
            var admin = await _context.Admins.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
            if (admin == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, admin.Password))
            {
                return null;
            }
            return admin;
        }

        public async Task<Admin> GetAdminById(int id)
        {
            return await _context.Admins.FindAsync(id);
        }

        public async Task<Admin> GetAdminByEmail(string email)
        {
            return await _context.Admins.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ICollection<Admin>> GetAllAdmins()
        {
            return await _context.Admins.ToListAsync();
        }

        public async Task UpdateAdminDetails(int id, AdminDto adminDto)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                admin.FirstName = adminDto.FirstName;
                admin.LastName = adminDto.LastName;
                admin.Email = adminDto.Email;
                admin.Password = adminDto.Password;
                admin.Pin = adminDto.Pin;

                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAdminBalance(int id, double newBalance)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                admin.Balance = newBalance;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<Admin> GetAdminByAccountNumber(long accountNumber)
        {
            return await _context.Admins.SingleOrDefaultAsync(u =>
                u.AccountNumber == accountNumber
            );
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ICollection<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task DeleteAdminAccount(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAccount(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
