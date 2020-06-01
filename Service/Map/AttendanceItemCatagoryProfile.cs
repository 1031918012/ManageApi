using AutoMapper;
using Domain;
using Infrastructure;

namespace Service
{
    public class AttendanceItemCatagoryProfile : Profile
    {
        public AttendanceItemCatagoryProfile()
        {
            CreateMap<AttendanceItemCatagory, AttendanceItemCatagoryDTO>()
                .ForMember(s => s.AttendanceItemCatagoryID, opt => opt.MapFrom(s => s.AttendanceItemCatagoryID))
                .ForMember(s => s.AttendanceItemCatagoryName, opt => opt.MapFrom(s => s.AttendanceItemCatagoryName))
                .ReverseMap();
        }
    }
}


