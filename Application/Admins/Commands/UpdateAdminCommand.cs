using Application.Dto;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;


namespace Application.Admins.Commands
{
    public class UpdateAdminCommand : IRequest<Admin>
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Pin { get; set; }
    }

    public class UpdateAdminCommandHandler : IRequestHandler<UpdateAdminCommand, Admin>
    {
        private readonly DataContext _context;

        public UpdateAdminCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<Admin> Handle(UpdateAdminCommand request, CancellationToken cancellationToken)
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
            }

            return admin;
        }
    }
}