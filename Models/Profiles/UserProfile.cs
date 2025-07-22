using AutoMapper;
using Mubank.Models;

namespace Mubank.Models
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {            
            CreateMap<UserCreateDTO, UserModel>();
            CreateMap<UserModel, UserResponseDTO>();
            CreateMap<UserModel, UserDTO>();
        }
    }
}
