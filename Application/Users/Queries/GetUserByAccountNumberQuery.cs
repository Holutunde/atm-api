using MediatR;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Application.Users.Queries
{
    public class GetUserByAccountNumberQuery : IRequest<User>
    {
        public long AccountNumber { get; set; }
    }

    public class GetUserByAccountNumberQueryHandler : IRequestHandler<GetUserByAccountNumberQuery, User>
    {
        private readonly IDataContext _context;

        public GetUserByAccountNumberQueryHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(GetUserByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.AccountNumber == request.AccountNumber, cancellationToken);
        }
    }
}
