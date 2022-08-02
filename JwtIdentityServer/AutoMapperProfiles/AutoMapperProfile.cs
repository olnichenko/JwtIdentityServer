using AutoMapper;
using DAL.Models;
using JwtIdentityServer.ViewModels;

namespace JwtIdentityServer.AutoMapperProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserVM, User>();
        }
    }
}
