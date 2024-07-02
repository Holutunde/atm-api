using Domain.Entities;
using Infrastructure.Data;
using MediatR;

namespace Application.Transactions.Queries
{
    public class GetTransactionByIdQuery : IRequest<Transaction>
    {
        public int Id { get; set; }
    }

    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Transaction>
    {
        private readonly DataContext _context;

        public GetTransactionByIdQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Transaction> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Transactions.FindAsync(new object[] { request.Id }, cancellationToken);
        }
    }
}
