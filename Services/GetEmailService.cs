using System.Security.Claims;

namespace ATMAPI.Services
{
    public class GetEmailService
    {
        public string GetEmailFromToken(ClaimsPrincipal user)
        {
            var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value;
            if (emailClaim == null)
            {
                throw new UnauthorizedAccessException("Email not found in token.");
            }

            return emailClaim;
        }

        public long GetAccountNumberFromToken(ClaimsPrincipal user)
        {
            var accountNumberClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (accountNumberClaim == null)
            {
                throw new UnauthorizedAccessException("Account number not found in token.");
            }

            return long.Parse(accountNumberClaim);
        }
    }
    
}
