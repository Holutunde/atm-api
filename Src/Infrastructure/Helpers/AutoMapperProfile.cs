using AutoMapper;
using Application.Dto;
using Domain.Entities;

namespace API.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Define your mappings here
            CreateMap<ApplicationUser, UpdateDetailsDto>().ReverseMap();
        }
    }
}