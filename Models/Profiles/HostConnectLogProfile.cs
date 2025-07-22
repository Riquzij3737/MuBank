using AutoMapper;

namespace Mubank.Models
{
    public class HostConnectLogProfile : Profile
    {
        public HostConnectLogProfile() => CreateMap<HostConnectLogModel, HostConnectLogModel>();        
    }
}
