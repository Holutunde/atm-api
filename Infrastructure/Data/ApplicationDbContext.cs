using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Application.Interfaces;

namespace Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IDataContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; } 
        
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
    
}
