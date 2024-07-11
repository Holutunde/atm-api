
namespace Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(string email, string role);
        string GenerateATmToken(long accountNumber);
    }
}