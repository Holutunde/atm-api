using Application.Common.ResultsModel;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries
{
    public class GetAllUsersQuery : IRequest<Result>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result>
    {
        private readonly IDataContext _context;

        public GetAllUsersQueryHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.Users.ToListAsync(cancellationToken);
            return Result.Success(users, $"Total users found: {users.Count}");
        }
    }
}
