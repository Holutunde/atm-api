using MediatR;
using Application.Interfaces;
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
        private readonly IDataContext _context;

        public GetUserByEmailQueryHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            return user;
        }
    }
}
