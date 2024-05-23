using System.Security.Claims;

namespace ATMAPI.Services
{
    public class GetAccountNumberService
    {
        public long GetAccountNumberFromToken(ClaimsPrincipal user)
        {
            var accountNumberClaim = user.FindFirst(ClaimTypes.Name)?.Value;
            if (accountNumberClaim == null)
            {
                throw new UnauthorizedAccessException("Account number not found in token.");
            }

            return long.Parse(accountNumberClaim);
        }
    }
}
