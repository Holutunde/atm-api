using Domain.Enum;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long AccountNumber { get; set; }
        public int Pin { get; set; }
        public double Balance { get; set; }
        public DateTime OpeningDate { get; set; }
        public string Role { get; set; } = Roles.User.ToString();
    }
}
