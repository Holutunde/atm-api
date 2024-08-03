using Application.Common.ResultsModel;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace Application.Users.Queries
{
    public class GetUserByIdQuery : IRequest<Result>
    {
        public string Id { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
            {
                return Result.Failure<GetUserByIdQuery>("User not found.");
            }

            return Result.Success(user);
        }
    }
}