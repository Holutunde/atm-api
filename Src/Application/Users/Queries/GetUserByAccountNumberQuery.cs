using Application.Common.ResultsModel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Users.Queries
{
    public class GetUserByAccountNumberQuery : IRequest<Result>
    {
        public long AccountNumber { get; set; }
    }

    public class GetUserByAccountNumberQueryHandler : IRequestHandler<GetUserByAccountNumberQuery, Result>
    {
        private readonly IDataContext _context;

        public GetUserByAccountNumberQueryHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetUserByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await _context.Users.SingleOrDefaultAsync(u => u.AccountNumber == request.AccountNumber, cancellationToken);
            if (user == null)
            {
                return Result.Failure<GetUserByAccountNumberQuery>("User not found.");
            }

            return Result.Success(user);
        }
    }
}