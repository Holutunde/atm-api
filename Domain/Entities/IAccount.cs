namespace Domain.Entities
{
    public interface IAccount
    {
        string Email { get; set; }
        string Password { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        int Pin { get; set; }
        double Balance { get; set; }
        long AccountNumber { get; set; }
        DateTime OpeningDate { get; set; }
        string Role { get; set; }
    }
}