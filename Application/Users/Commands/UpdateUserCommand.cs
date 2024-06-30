using MediatR;
using Application.Dto;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands
{
    public class UpdateUserCommand : IRequest<User>
    {
        public int Id { get; set; }
        public UserDto UserDto { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User>
    {
        private readonly DataContext _context;

        public UpdateUserCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.Id);

            if (user != null)
            {
                user.FirstName = request.UserDto.FirstName;
                user.LastName = request.UserDto.LastName;
                user.Email = request.UserDto.Email;
                user.Password = request.UserDto.Password;
                user.Pin = request.UserDto.Pin;

                _context.Users.Update(user);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return user;
        }
    }
}
