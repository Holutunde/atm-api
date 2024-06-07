using ATMAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace ATMAPI.Data
{

    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id); 

            modelBuilder.Entity<Admin>().HasKey(u => u.Id); 
        }

        
    }
}