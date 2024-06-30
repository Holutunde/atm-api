using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Admins.Queries
{
    public class GetAdminByAccountNumberQuery : IRequest<Admin>
    {
        public long AccountNumber { get; set; }
    }

    public class GetAdminByAccountNumberQueryHandler : IRequestHandler<GetAdminByAccountNumberQuery, Admin>
    {
        private readonly DataContext _context;

        public GetAdminByAccountNumberQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Admin> Handle(GetAdminByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            return await _context.Admins.SingleOrDefaultAsync(u => u.AccountNumber == request.AccountNumber, cancellationToken);
        }
    }
}
