using Application.Common.ResultsModel;
using Application.Interfaces;
using MediatR;


namespace Application.Admins.Commands
{
    public class UpdateAdminBalanceCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public double NewBalance { get; set; }
    }

    public class UpdateAdminBalanceCommandHandler(IDataContext context) : IRequestHandler<UpdateAdminBalanceCommand, Result>
    {
        private readonly IDataContext _context = context;

        public async Task<Result> Handle(UpdateAdminBalanceCommand request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
            if (admin != null)
            {
                admin.Balance = request.NewBalance;
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success<UpdateAdminBalanceCommand>( "Admin balance updated successfully.", admin);
            }

            return Result.Failure<UpdateAdminBalanceCommand>($"Admin with ID {request.Id} not found.");
        }
    }
}
