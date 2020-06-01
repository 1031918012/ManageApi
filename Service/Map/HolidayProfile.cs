using AutoMapper;
using Domain;
using Infrastructure;

namespace Service
{
    public class HolidayProfile : Profile
    {
        public HolidayProfile()
        {
            CreateMap<Holiday, HolidayDTO>()
                .ForMember(s => s.HolidayID, opt => opt.MapFrom(s => s.HolidayID))
                .ForMember(s => s.HolidayName, opt => opt.MapFrom(s => s.HolidayName))
                .ForMember(s => s.HolidayYear, opt => opt.MapFrom(s => s.HolidayYear))
                .ForMember(s => s.HolidayNumber, opt => opt.MapFrom(s => s.HolidayNumber))
                .ForMember(s => s.CreateTime, opt => opt.MapFrom(s => s.CreateTime))
                .ForMember(s => s.StartHolidayTime, opt => opt.MapFrom(s => s.StartHolidayTime))
                .ForMember(s => s.EndHolidayTime, opt => opt.MapFrom(s => s.EndHolidayTime))
                .ReverseMap();
        }
    }
}
