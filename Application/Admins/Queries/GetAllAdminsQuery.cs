using MediatR;
using System.Collections.Generic;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Admins.Queries
{
    public class GetAllAdminsQuery : IRequest<List<Admin>>
    {
    }

    public class GetAllAdminsQueryHandler : IRequestHandler<GetAllAdminsQuery, List<Admin>>
    {
        private readonly DataContext _context;

        public GetAllAdminsQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Admin>> Handle(GetAllAdminsQuery request, CancellationToken cancellationToken)
        {
            // Retrieve all admins from the database
            var admins = await _context.Admins.ToListAsync(cancellationToken);
            return admins;
        }
    }
}
