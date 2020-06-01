using AutoMapper;
using Domain;
using Infrastructure;

namespace Service
{
    public class AttendanceItemUnitProfile : Profile
    {
        public AttendanceItemUnitProfile()
        {
            CreateMap<AttendanceItemUnit, AttendanceItemUnitDTO>()
                .ForMember(s => s.AttendanceItemUnitID, opt => opt.MapFrom(s => s.AttendanceItemUnitID))
                .ForMember(s => s.AttendanceItemUnitName, opt => opt.MapFrom(s => s.AttendanceItemUnitName))
                .ForMember(s => s.AttendanceItemID, opt => opt.MapFrom(s => s.AttendanceItemID))
                .ForMember(s => s.AttendanceItemName, opt => opt.MapFrom(s => s.AttendanceItemName))
                .ForMember(s => s.IsSelect, opt => opt.MapFrom(s => true))
                .ReverseMap();
        }
    }
}


