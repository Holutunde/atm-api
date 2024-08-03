using Application.Common.ResultsModel;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries
{
    public class GetAllUsersByUserRoleQuery : IRequest<Result>
    {
    }

    public class GetAllUsersByUserRoleQueryHandler : IRequestHandler<GetAllUsersByUserRoleQuery, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersByUserRoleQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(GetAllUsersByUserRoleQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.Where(u => u.Role == Roles.User).ToListAsync(cancellationToken);

            if (!users.Any())
            {
                return Result.Failure<GetAllUsersByUserRoleQuery>("No users found with the role 'User'.");
            }

            return Result.Success(new   
            {  
                users,   
                totalUsers = users.Count   
            });
        }
    }
}