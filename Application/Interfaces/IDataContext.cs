using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Application.Interfaces
{
    public interface IDataContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Admin> Admins { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}