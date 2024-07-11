using MediatR;
using Domain.Entities;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Common.ResultsModel;

namespace Application.Admins.Queries
{
    public class GetAllAdminsQuery : IRequest<Result>
    {
        public string? SearchValue { get; set; }
    }

    public class GetAllAdminsQueryHandler : IRequestHandler<GetAllAdminsQuery, Result>
    {
        private readonly IDataContext _context;

        public GetAllAdminsQueryHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetAllAdminsQuery request, CancellationToken cancellationToken)
        {
            var admins = await _context.Admins.ToListAsync(cancellationToken);

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                admins = FilterByName(admins, request.SearchValue);
            }

            var resultData = new
            {
                Admins = admins,
                Count = admins.Count
            };

            return Result.Success(resultData, $"Total admins found: {admins.Count}");
        }

        private static List<Admin> FilterByName(List<Admin> admins, string searchValue)
        {
            return admins.Where(u => u.FirstName.Contains(searchValue) || u.LastName.Contains(searchValue)).ToList();
        }
    }
}