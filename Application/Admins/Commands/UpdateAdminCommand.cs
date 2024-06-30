using Application.Dto;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;


namespace Application.Admins.Commands
{
    public class UpdateAdminCommand : IRequest<Admin>
    {
        public int Id { get; set; }
        public AdminDto AdminDto { get; set; }
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
                admin.FirstName = request.AdminDto.FirstName;
                admin.LastName = request.AdminDto.LastName;
                admin.Email = request.AdminDto.Email;
                admin.Password = request.AdminDto.Password;
                admin.Pin = request.AdminDto.Pin;

                _context.Admins.Update(admin);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return admin;
        }
    }
}