using Application.Common.ResultsModel;
using Application.Dto;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using AutoMapper;


namespace Application.Users.Commands
{
    public class UpdateUserDetailsCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public UpdateDetailsDto UpdateDetailsDto { get; set; }
    }

    public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateUserDetailsCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UpdateUserDetailsCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Result> Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Result.Failure<UpdateUserDetailsCommand>("User not found.");
            }

            if (request.UpdateDetailsDto == null)
            {
                return Result.Failure<UpdateUserDetailsCommand>("Update details are required.");
            }

            user.UserName = request.UpdateDetailsDto.UserName;
            user.FirstName = request.UpdateDetailsDto.FirstName;
            user.LastName = request.UpdateDetailsDto.LastName;

            IdentityResult updateResult = await _userManager.UpdateAsync(user);
            if (updateResult.Succeeded)
            {
                var updatedUserDetails = _mapper.Map<UpdateDetailsDto>(user);
                return Result.Success<UpdateUserDetailsCommand>("User details updated successfully.", updatedUserDetails);
            }

            return Result.Failure<UpdateUserDetailsCommand>("Failed to update user details.");
        }
    }
}