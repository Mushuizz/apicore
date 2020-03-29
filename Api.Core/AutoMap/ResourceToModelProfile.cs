using Api.Core.Model.User;
using AutoMapper;

namespace Api.Core.AutoMap
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<UserCredentials, UserEntity>();
            CreateMap<UserEntity, UserCredentials>();
        }
    }
}
