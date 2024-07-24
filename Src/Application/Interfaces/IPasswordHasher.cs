
namespace Application.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);

        bool VerifyPassword(string enteredPassword, string storedHashedPassword);

    }
}