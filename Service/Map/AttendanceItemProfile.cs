using AutoMapper;
using Domain;
using Infrastructure;

namespace Service
{
    public class AttendanceItemProfile : Profile
    {
        public AttendanceItemProfile()
        {
            CreateMap<AttendanceItem, AttendanceItemDTO>()
                .ForMember(s => s.AttendanceItemID, opt => opt.MapFrom(s => s.AttendanceItemID))
                .ForMember(s => s.AttendanceItemName, opt => opt.MapFrom(s => s.AttendanceItemName))
                .ForMember(s => s.AttendanceItemCatagoryID, opt => opt.MapFrom(s => s.AttendanceItemCatagoryID))
                .ForMember(s => s.AttendanceItemCatagoryName, opt => opt.MapFrom(s => s.AttendanceItemCatagoryName))
                .ForMember(s => s.IsEnable, opt => opt.MapFrom(s => true))
                .ReverseMap();
        }
    }
}

