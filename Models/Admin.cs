namespace ATMAPI.Models
{
    public class Admin
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public long AccountNumber { get; set; }
        public int Pin { get; set; }
        public double Balance { get; set; }
        public DateTime OpeningDate { get; set; }
        public string Role { get; set; } = "Admin";
    }
}