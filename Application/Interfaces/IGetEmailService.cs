using System.Security.Claims;

namespace Application.Interfaces
{
    public interface IGetEmailService
    {
        string GetEmailFromToken(ClaimsPrincipal user);
        long GetAccountNumberFromToken(ClaimsPrincipal user);
    }
}