using Application.Common.ResultsModel;
using Application.Interfaces;
using MediatR;


namespace Application.Admins.Commands
{
    public class ChangeAdminPinCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public int NewPin { get; set; }
    }

    public class ChangeAdminPinCommandHandler(IDataContext context) : IRequestHandler<ChangeAdminPinCommand, Result>
    {
        private readonly IDataContext _context = context;

        public async Task<Result> Handle(ChangeAdminPinCommand request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.FindAsync(request.Id);
            if (admin != null)
            {
                admin.Pin = request.NewPin;
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success(admin, "Admin PIN changed successfully.");
            }

            return Result.Failure<ChangeAdminPinCommand>($"Admin with ID {request.Id} not found.");
        }
    }
}
