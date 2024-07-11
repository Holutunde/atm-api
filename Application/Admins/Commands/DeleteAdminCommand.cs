using Application.Common.ResultsModel;
using Application.Interfaces;
using MediatR;


namespace Application.Admins.Commands
{
    public class DeleteAdminCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class DeleteAdminCommandHandler(IDataContext context) : IRequestHandler<DeleteAdminCommand, Result>
    {
        private readonly IDataContext _context = context;

        public async Task<Result> Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.FindAsync(request.Id);

            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success($"Admin with ID {request.Id} deleted successfully.");
            }

            return Result.Failure<DeleteAdminCommand>($"Admin with ID {request.Id} not found.");
        }
    }
}
