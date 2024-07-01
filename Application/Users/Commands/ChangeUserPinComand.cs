using MediatR;
using Infrastructure.Data;


namespace Application.Users.Commands
{
    public class ChangeUserPinCommand : IRequest<int>
    {
        public int Id { get; set; }
        public int NewPin { get; set; }
    }

    public class ChangeUserPinCommandHandler : IRequestHandler<ChangeUserPinCommand, int>
    {
        private readonly DataContext _context;

        public ChangeUserPinCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(ChangeUserPinCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user != null)
            {
                user.Pin = request.NewPin;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return user.Pin;
        }
    }
}