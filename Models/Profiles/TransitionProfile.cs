using AutoMapper;

namespace Mubank.Models
{
    public class TransitionProfile : Profile
    {
        public TransitionProfile() => CreateMap<TransationsModel, TransitionsDTO>();        
    }
}
