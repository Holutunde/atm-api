using Application.Common.ResultsModel;
using Application.Interfaces;
using MediatR;


namespace Application.Users.Queries
{
    public class GetUserByIdQuery : IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result>
    {
        private readonly IDataContext _context;

        public GetUserByIdQueryHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user != null)
            {
                return Result.Success(user, "User found.");
            }
            return Result.Failure<GetUserByIdQuery>($"User with ID {request.Id} not found.");
        }
    }
}
