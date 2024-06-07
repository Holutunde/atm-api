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
    }
}
