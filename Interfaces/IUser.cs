
namespace ATMAPI.Interfaces
{        public interface IUser
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public long AccountNumber { get; set; }
        public int Pin { get; set; }
        public double Balance { get; set; }
        public DateTime OpeningDate { get; set; }
    }
}