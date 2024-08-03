using Application.Common.ResultsModel;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Users.Queries
{
    public class GetUserByEmailQuery : IRequest<Result>
    {
        public string Email { get; set; }
    }

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserByEmailQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<GetUserByEmailQuery>("User not found.");
            }

            return Result.Success(user);
        }
    }
}