
namespace Application.Interfaces
{
    public interface IPinHasher
    {
        string HashPin(int pin);

        bool VerifyPin(int enteredPin, string storedHashedPassword);

    }
}