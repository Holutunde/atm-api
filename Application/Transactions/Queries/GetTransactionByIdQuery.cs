using Domain.Entities;
using Application.Interfaces;
using MediatR;

namespace Application.Transactions.Queries
{
    public class GetTransactionByIdQuery : IRequest<Transaction>
    {
        public int Id { get; set; }
    }

    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Transaction>
    {
        private readonly IDataContext _context;

        public GetTransactionByIdQueryHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Transaction> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Transactions.FindAsync(new object[] { request.Id }, cancellationToken);
        }
    }
}
