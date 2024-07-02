using Domain.Entities;
using Infrastructure.Data;
using MediatR;

namespace Application.Transactions.Commands
{
    public class CreateTransactionCommand : IRequest
    {
        public Transaction Transaction { get; set; }
    }

    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand>
    {
        private readonly DataContext _context;

        public CreateTransactionCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            _context.Transactions.AddAsync(request.Transaction);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
