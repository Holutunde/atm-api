using MediatR;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Application.Users.Queries
{
    public class GetUserByEmailQuery : IRequest<User>
    {
        public string Email { get; set; }
    }

    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, User>
    {
        private readonly DataContext _context;

        public GetUserByEmailQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        }
    }
}
