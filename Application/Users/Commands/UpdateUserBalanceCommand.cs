using MediatR;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Application.Users.Commands
{
    public class UpdateUserBalanceCommand : IRequest<double>
    {
        public int Id { get; set; }
        public double NewBalance { get; set; }
    }

    public class UpdateUserBalanceCommandHandler : IRequestHandler<UpdateUserBalanceCommand, double>
    {
        private readonly DataContext _context;

        public UpdateUserBalanceCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<double> Handle(UpdateUserBalanceCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user != null)
            {
                user.Balance = request.NewBalance;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return user.Balance;
        }
    }
}
