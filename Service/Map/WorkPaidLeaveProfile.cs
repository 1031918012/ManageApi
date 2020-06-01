using AutoMapper;
using Domain;
using Infrastructure;

namespace Service
{
    public class WorkPaidLeaveProfile : Profile
    {
        public WorkPaidLeaveProfile()
        {
            //CreateMap<WorkPaidLeave, WorkPaidLeaveDTO>()
            //    .ForMember(s => s.WorkPaidLeaveID, opt => opt.MapFrom(s => s.WorkPaidLeaveID))
            //    .ForMember(s => s.PaidLeaveTime, opt => opt.MapFrom(s => s.PaidLeaveTime))
            //    .ForMember(s => s.Type, opt => opt.MapFrom(s => s.Type.WorkPaidLeaveTypeEnumTo()))
            //    .ForMember(s => s.HolidayID, opt => opt.MapFrom(s => s.HolidayID))
            //    .ForMember(s => s.HolidayName, opt => opt.MapFrom(s => s.HolidayName))
            //    .ReverseMap();
        }
    }
}
