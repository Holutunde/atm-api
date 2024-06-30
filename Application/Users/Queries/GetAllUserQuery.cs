using MediatR;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries
{
    public class GetAllUsersQuery : IRequest<List<User>>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<User>>
    {
        private readonly DataContext _context;

        public GetAllUsersQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<List<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            // Retrieve all users from the database
            var users = await _context.Users.ToListAsync(cancellationToken);
            return users;
        }
    }
}
