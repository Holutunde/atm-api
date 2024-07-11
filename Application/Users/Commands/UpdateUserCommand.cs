using MediatR;
using Application.Interfaces;
using Application.Common.ResultsModel;

namespace Application.Users.Commands
{
    public class UpdateUserCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Pin { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly IDataContext _context;

        public UpdateUserCommandHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user == null)
            {
                return Result.Failure<UpdateUserBalanceCommand>($"User with id {request.Id} not found.");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Password = request.Password;
            user.Pin = request.Pin;

            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(user, "User details updated successfully.");
        }
    }

}
