using Application.Common.ResultsModel;
using Application.Interfaces;
using MediatR;


namespace Application.Admins.Commands
{
    public class UpdateAdminCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Pin { get; set; }
    }

    public class UpdateAdminCommandHandler(IDataContext context) : IRequestHandler<UpdateAdminCommand, Result>
    {
        private readonly IDataContext _context = context;

        public async Task<Result> Handle(UpdateAdminCommand request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.FindAsync(request.Id);

            if (admin != null)
            {
                admin.FirstName = request.FirstName;
                admin.LastName = request.LastName;
                admin.Email = request.Email;
                admin.Password = request.Password;
                admin.Pin = request.Pin;

                _context.Admins.Update(admin);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success(admin, "Admin details updated successfully.");
            }

            return Result.Failure<UpdateAdminCommand>($"Admin with ID {request.Id} not found.");
        }
    }
}
