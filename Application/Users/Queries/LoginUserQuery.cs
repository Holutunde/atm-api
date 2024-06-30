using MediatR;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Application.Dto;
using BCrypt.Net;

namespace Application.Users.Queries
{
    public class LoginUserQuery : IRequest<User>
    {
        public OnlineLoginDto LoginDto { get; set; }
    }

    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, User>
    {
        private readonly DataContext _context;

        public LoginUserQueryHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.LoginDto.Email, cancellationToken);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, user.Password))
            {
                return null;
            }
            return user;
        }
    }
}
