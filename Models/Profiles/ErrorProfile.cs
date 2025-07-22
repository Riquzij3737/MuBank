using AutoMapper;

namespace Mubank.Models
{
    public class ErrorProfile : Profile
    {
        public ErrorProfile() => CreateMap<ErrorModel, ErrorDTO>();
    }
}
