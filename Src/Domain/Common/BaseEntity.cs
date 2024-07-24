

using System.ComponentModel.DataAnnotations;
using Domain.Enum;

namespace Domain.Entities
{
    
    public interface IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public  string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public int Pin { get; set; }
        
        public double Balance { get; set; }
        
        public long AccountNumber { get; set; }
        
        public DateTime OpeningDate { get; set; }
        
        public Roles Role { get; set; }
        
        public string RoleDesc { get; set; }
        
        // public DateTime StartTime { get; set; }
        //
        // public DateTime EndTime { get; set; }
        //
        // public string Request { get; set; }
        //
        // public string Response { get; set; }
    }
    public class BaseEntity : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        public string Email { get; set; }
        
        public string Password { get; set; }
        
        public  string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public int Pin { get; set; }
        
        public double Balance { get; set; }
        
        public long AccountNumber { get; set; }
        
        public DateTime OpeningDate { get; set; }
        
        public Roles Role { get; set; }
        
        public string RoleDesc { get; set; }
        
        // public DateTime StartTime { get; set; }
        //
        // public DateTime EndTime { get; set; }
        //
        // public string Request { get; set; }
        //
        // public string Response { get; set; }
    }
    
    
}