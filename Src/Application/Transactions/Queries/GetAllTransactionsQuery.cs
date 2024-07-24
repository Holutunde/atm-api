using Domain.Entities;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Transactions.Queries
{
    public class GetAllTransactionsQuery : IRequest<List<Transaction>>
    {
    }

    public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, List<Transaction>>
    {
        private readonly IDataContext _context;

        public GetAllTransactionsQueryHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Transactions.ToListAsync(cancellationToken);
        }
    }
}
