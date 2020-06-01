using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class AttendanceGroupProfile : Profile
    {
        public AttendanceGroupProfile()
        {
            CreateMap<AttendanceGroup, AttendanceGroupDTO>()
                .ForMember(s => s.AttendanceGroupID, opt => opt.MapFrom(s => s.AttendanceGroupID))
                .ForMember(s => s.AttendanceRuleID, opt => opt.MapFrom(s => s.AttendanceRuleID))
                .ForMember(s => s.ClockInWay, opt => opt.MapFrom(s => (int)s.ClockInWay))
                .ForMember(s => s.IsDynamicRowHugh, opt => opt.MapFrom(s => s.IsDynamicRowHugh))
                .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(s => s.ShiftTypeConvert, opt => opt.MapFrom(s => s.ShiftType.GetDescription()))
                .ForMember(s => s.ShiftType, opt => opt.MapFrom(s => (int)s.ShiftType))
                .ForMember(s => s.Range, opt => opt.MapFrom(s => s.Range))
                .ForMember(s => s.OvertimeID, opt => opt.MapFrom(s => s.OvertimeID));

            CreateMap<AttendanceGroupDTO, AttendanceGroup>()
                .ForMember(s => s.AttendanceGroupID, opt => opt.MapFrom(s => s.AttendanceGroupID))
                .ForMember(s => s.AttendanceRuleID, opt => opt.MapFrom(s => s.AttendanceRuleID))
                .ForMember(s => s.ClockInWay, opt => opt.MapFrom(s => s.ClockInWay))
                .ForMember(s => s.IsDynamicRowHugh, opt => opt.MapFrom(s => s.IsDynamicRowHugh))
                .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(s => s.ShiftType, opt => opt.MapFrom(s => s.ShiftType))
                .ForMember(s => s.Range, opt => opt.MapFrom(s => s.Range))
                .ForMember(s => s.OvertimeID, opt => opt.MapFrom(s => s.OvertimeID));
        }
    }
}
