using Domain.Enum;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser, IBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public Guid GuId { get; set; } = Guid.NewGuid();
        
        public  string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string PinHash { get; set; }
        
        public double Balance { get; set; }
        
        public long AccountNumber { get; set; }
        
        public DateTime OpeningDate { get; set; }

        public Roles Role { get; set; }

        public string RoleDesc { get; set; }
        
        public Status UserStatus { get; set; }
        
        public string UserStatusDes { get; set; }
        
    }
}