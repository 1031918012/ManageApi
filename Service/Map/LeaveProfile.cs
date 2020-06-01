using AutoMapper;
using Domain;
using Infrastructure;

namespace Service
{
    public class LeaveProfile : Profile
    {
        public LeaveProfile()
        {
            CreateMap<Leave, LeaveDTO>()
                .ForMember(s => s.LeaveID, opt => opt.MapFrom(s => s.LeaveID))
                .ForMember(s => s.Department, opt => opt.MapFrom(s => s.Department))
                .ForMember(s => s.IDCard, opt => opt.MapFrom(s => s.IDCard))
                .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(s => s.JobNumber, opt => opt.MapFrom(s => s.JobNumber))
                .ForMember(s => s.CompanyID, opt => opt.MapFrom(s => s.CompanyID))
                .ForMember(s => s.CompanyName, opt => opt.MapFrom(s => s.CompanyName))
                .ForMember(s => s.StartTime, opt => opt.MapFrom(s => s.StartTime))
                .ForMember(s => s.EndTime, opt => opt.MapFrom(s => s.EndTime))
                .ForMember(s => s.LeaveName, opt => opt.MapFrom(s => s.LeaveName))
                .ReverseMap();
        }
    }
}
