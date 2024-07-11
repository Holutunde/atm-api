using Application.Common.ResultsModel;
using Application.Interfaces;
using MediatR;


namespace Application.Admins.Queries
{
    public class GetAdminByIdQuery : IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class GetAdminByIdQueryHandler : IRequestHandler<GetAdminByIdQuery, Result>
    {
        private readonly IDataContext _context;

        public GetAdminByIdQueryHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetAdminByIdQuery request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.FindAsync(request.Id);
            if (admin != null)
            {
                return Result.Success(admin, "Admin found.");
            }
            return Result.Failure<GetAdminByIdQuery>($"Admin with ID {request.Id} not found.");
        }
    }
}
