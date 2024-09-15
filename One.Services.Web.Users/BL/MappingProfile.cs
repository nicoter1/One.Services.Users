using AutoMapper;
using One.Models.Users;
using One.Models.Users.Payloads; 

namespace One.Services.Web.Users.BL
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, One.Database.Users.Models.User >().ReverseMap() ;
            CreateMap<UserPayload, One.Database.Users.Models.User>().ReverseMap();
        }
    }
}