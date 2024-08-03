using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ATMAPI.Tests.Mocks
{
    public static class SignInManagerMock
    {
        public static Mock<SignInManager<TUser>> CreateMockSignInManager<TUser>(UserManager<TUser> userManager) where TUser : class
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
            return new Mock<SignInManager<TUser>>(
                userManager,
                contextAccessor.Object,
                claimsFactory.Object,
                null,
                null,
                null,
                null
            );
        }
    }
}