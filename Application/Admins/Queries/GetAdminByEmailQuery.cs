using Domain.Entities;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Admins.Queries
{
    public class GetAdminByEmailQuery : IRequest<Admin>
    {
        public required string Email { get; set; }
    }

    public class GetAdminByEmailQueryHandler(IDataContext context) : IRequestHandler<GetAdminByEmailQuery, Admin>
    {
        private readonly IDataContext _context = context;

        public async Task<Admin> Handle(GetAdminByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _context.Admins.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        }
    }
}
