using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Admins.Queries
{
    public class GetAdminByEmailQuery : IRequest<Admin>
    {
        public string Email { get; set; }
    }

    public class GetAdminByEmailQueryHandler : IRequestHandler<GetAdminByEmailQuery, Admin>
    {
        private readonly DataContext _context;

        public GetAdminByEmailQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Admin> Handle(GetAdminByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _context.Admins.SingleOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        }
    }
}
