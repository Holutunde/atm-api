using Domain.Entities;
using Infrastructure.Data;
using MediatR;


namespace Application.Admins.Queries
{
    public class GetAdminByIdQuery : IRequest<Admin>
    {
        public int Id { get; set; }
    }

    public class GetAdminByIdQueryHandler : IRequestHandler<GetAdminByIdQuery, Admin>
    {
        private readonly DataContext _context;

        public GetAdminByIdQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Admin> Handle(GetAdminByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Admins.FindAsync(request.Id);
        }
    }
}
