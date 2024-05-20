using AutoMapper;
using ATMAPI.Dto;
using ATMAPI.Models;


namespace ATMAPI.Helper 
{
    public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
        CreateMap<Account, AccountUpdateDto>();
        CreateMap<Account, AccountDto>();
        CreateMap<AccountDto, Account>();
    }
  }

}