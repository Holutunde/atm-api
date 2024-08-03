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
    public class GetAllUsersByAdminRoleQuery : IRequest<Result>
    {
    }

    public class GetAllUsersByAdminRoleQueryHandler : IRequestHandler<GetAllUsersByAdminRoleQuery, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersByAdminRoleQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(GetAllUsersByAdminRoleQuery request, CancellationToken cancellationToken)
        {
            var admins = await _userManager.Users.Where(u => u.Role == Roles.Admin).ToListAsync(cancellationToken);

            if (!admins.Any())
            {
                return Result.Failure<GetAllUsersByAdminRoleQuery>("No users found with the role 'Admin'.");
            }

            return Result.Success(new   
            {  
                admins,   
                totalAdmins = admins.Count   
            });  
        }
    }
}