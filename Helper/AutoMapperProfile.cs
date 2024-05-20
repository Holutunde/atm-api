using AutoMapper;
using ATMAPI.Dto;
using ATMAPI.Models;


namespace ATMAPI.Helper 
{
    public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
        CreateMap<Account, AccountDto>().ReverseMap();
        CreateMap<Account, AccountUpdateDto>().ReverseMap();
    }
  }

}