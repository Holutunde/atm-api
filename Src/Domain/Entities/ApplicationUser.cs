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
        
        public required  string FirstName { get; set; }
        
        public required string LastName { get; set; }
        
        public required string PinHash { get; set; }
        
        public double Balance { get; set; }
        
        public long AccountNumber { get; set; }
        
        public DateTime OpeningDate { get; set; }

        public Roles Role { get; set; }

        public required string RoleDesc { get; set; }
        
        public Status UserStatus { get; set; }
        
        public required string UserStatusDes { get; set; }
        
    }
}