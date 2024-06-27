
using Infrastructure.Data;
using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Register(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Login(OnlineLoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return null;
            }
            return user;
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ICollection<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }


        public async Task<User> GetUserByAccountNumber(long accountNumber)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.AccountNumber == accountNumber);
        }
        public async Task UpdateUserDetails(int id, UserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.Email = userDto.Email;
                user.Password = userDto.Password;
                user.Pin = userDto.Pin;

                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateUserBalance(int id, double newBalance)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Balance = newBalance;

                await _context.SaveChangesAsync();
            }
        }


        public async Task<ICollection<User>> GetAllUsers()
        {
            return await Task.Run(() => _context.Users.ToList());
        }
        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
