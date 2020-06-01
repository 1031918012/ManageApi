using AutoMapper;
using Domain;
using Infrastructure;

namespace Service
{
    public class BreakoffDetailProfile : Profile
    {
        public BreakoffDetailProfile()
        {
            CreateMap<BreakoffDetail, BreakoffDetailPageDTO>()
                .ForMember(s => s.ChangeTime, opt => opt.MapFrom(s => s.ChangeTime))
                .ForMember(s => s.CurrentQuota, opt => opt.MapFrom(s => s.CurrentQuota))
                .ForMember(s => s.CreateTime, opt => opt.MapFrom(s => s.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") ))
                .ForMember(s => s.Remark, opt => opt.MapFrom(s =>s.Remark ))
                .ForMember(s => s.IDCard, opt => opt.MapFrom(s =>s.IDCard ))
                .ForMember(s => s.CompanyID, opt => opt.MapFrom(s =>s.CompanyID ))
                .ForMember(s => s.ChangeType, opt => opt.MapFrom(s => s.ChangeType.GetDescription()))
                .ReverseMap();
        }
    }
}
