
namespace Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string email, string role);
        string GenerateATmToken(long accountNumber);
    }
}