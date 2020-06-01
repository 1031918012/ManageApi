using AutoMapper;
using Domain;
using Infrastructure;

namespace Service
{
    public class AttendanceMonthlyRecordProfile : Profile
    {
        //public readonly ISerializer<string> _serializer;
        public AttendanceMonthlyRecordProfile()
        {

            CreateMap<AttendanceMonthlyRecord, AttendanceMonthlyRecordDTO>()
                .ForMember(s => s.AttendanceMonthlyRecordID, opt => opt.MapFrom(s => s.AttendanceMonthlyRecordID))
                .ForMember(s => s.AttendanceDate, opt => opt.MapFrom(s => s.AttendanceDate))
                //.ForMember(s => s.AttendanceProjects, opt => opt.MapFrom(s => _serializer.Desrialize<string, List<AttendanceItemForComputDTO>>(s.AttendanceProjectsJson)))
                .ForMember(s => s.AttendanceProjects, opt => opt.MapFrom(s => s.AttendanceProjects))
                .ForMember(s => s.CompanyID, opt => opt.MapFrom(s => s.CompanyID))
                .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(s => s.CompanyName, opt => opt.MapFrom(s => s.CompanyName))
                .ForMember(s => s.Department, opt => opt.MapFrom(s => s.Department))
                .ForMember(s => s.EmployeeNo, opt => opt.MapFrom(s => s.EmployeeNo))
                .ForMember(s => s.IDCard, opt => opt.MapFrom(s => s.IDCard))
                .ForMember(s => s.Position, opt => opt.MapFrom(s => s.Position))
                .ReverseMap();
        }
        //public AttendanceMonthlyRecordProfile(ISerializer<string> serializer)
        //{
        //    _serializer = serializer;
        //}
    }
}
