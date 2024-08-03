using Microsoft.AspNetCore.Identity;  
using Moq;  

namespace ATMAPI.Tests.Mock  
{  
    public static class UserManagerMock  
    {  
        public static Mock<UserManager<TUser>> CreateMockUserManager<TUser>() where TUser : class  
        {  
            var store = new Mock<IUserStore<TUser>>();  
            var userManagerMock = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);  
            return userManagerMock;  
        }  
    }  
}