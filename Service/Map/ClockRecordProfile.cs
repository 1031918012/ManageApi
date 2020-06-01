using AutoMapper;
using Domain;
using Infrastructure;

namespace Service
{
    public class ClockRecordProfile : Profile
    {
        public ClockRecordProfile()
        {
            CreateMap<ClockRecord, ClockRecordDTO>()
                .ForMember(s => s.ClockRecordID, opt => opt.MapFrom(s => s.ClockRecordID))
                .ForMember(s => s.IDCard, opt => opt.MapFrom(s => s.IDCard))
                .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(s => s.EmployeeNo, opt => opt.MapFrom(s => s.EmployeeNo))
                .ForMember(s => s.Department, opt => opt.MapFrom(s => s.Department))
                .ForMember(s => s.Position, opt => opt.MapFrom(s => s.Position))
                .ForMember(s => s.CompanyID, opt => opt.MapFrom(s => s.CompanyID))
                .ForMember(s => s.CompanyName, opt => opt.MapFrom(s => s.CompanyName))
                .ForMember(s => s.AttendanceDate, opt => opt.MapFrom(s => s.AttendanceDate.ToString("yyyy-MM-dd")))
                .ForMember(s => s.ClockTime, opt => opt.MapFrom(s => s.ClockTime.ToString("yyyy-MM-dd HH:mm:ss")))
                .ForMember(s => s.ClockType, opt => opt.MapFrom(s => s.ClockType.GetDescription()))
                .ForMember(s => s.ClockResult, opt => opt.MapFrom(s => s.ClockResult.GetDescription()))
                .ForMember(s => s.ClockWay, opt => opt.MapFrom(s => s.ClockWay.GetDescription()))
                .ForMember(s => s.IsInRange, opt => opt.MapFrom(s => s.IsInRange))
                .ForMember(s => s.CustomerID, opt => opt.MapFrom(s => s.CustomerID))
                .ForMember(s => s.DepartmentID, opt => opt.MapFrom(s => s.DepartmentID))
                .ForMember(s => s.Location, opt => opt.MapFrom(s => s.Location))
                .ForMember(s => s.Remark, opt => opt.MapFrom(s => s.Remark))
                .ForMember(s => s.AbnormalReason, opt => opt.MapFrom(s => s.AbnormalReason))
                .ForMember(s => s.ClockImage1, opt => opt.MapFrom(s => s.ClockImage1))
                .ForMember(s => s.ClockImage2, opt => opt.MapFrom(s => s.ClockImage2))
                .ForMember(s => s.ClockDevice, opt => opt.MapFrom(s => s.ClockDevice))
                .ForMember(s => s.Latitude, opt => opt.MapFrom(s => s.Latitude))
                .ForMember(s => s.Longitude, opt => opt.MapFrom(s => s.Longitude))
                .ForMember(s => s.IsFieldAudit, opt => opt.MapFrom(s => s.IsFieldAudit == FieldAuditEnum.Checked))
                .ReverseMap();
        }
    }
}
