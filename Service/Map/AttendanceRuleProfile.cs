using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class AttendanceRuleProfile : Profile
    {
        public AttendanceRuleProfile()
        {
            CreateMap<AttendanceRule, AttendanceRuleDTO>()
                .ForMember(s => s.AttendanceRuleID, opt => opt.MapFrom(s => s.AttendanceRuleID))
                .ForMember(s => s.RuleName, opt => opt.MapFrom(s => s.RuleName))
                .ForMember(s => s.Remark, opt => opt.MapFrom(s => s.Remark))
                .ForMember(s => s.CompanyID, opt => opt.MapFrom(s => s.CompanyID))
                .ForMember(s => s.CompanyName, opt => opt.MapFrom(s => s.CompanyName))
                .ForMember(s => s.CreateTime, opt => opt.MapFrom(s => s.CreateTime.ToString("yyyy年MM月dd日")))
                .ForMember(s => s.Creator, opt => opt.MapFrom(s => s.Creator))
                .ForMember(s => s.EarlyLeaveRule, opt => opt.MapFrom(s => s.EarlyLeaveRule))
                .ForMember(s => s.LateRule, opt => opt.MapFrom(s => s.LateRule))
                .ForMember(s => s.AbsenceRule, opt => opt.MapFrom(s => s.NotClockRule))
                .ReverseMap();

            CreateMap<AttendanceRule, AttendanceRuleUpdateDTO>()
                .ForMember(s => s.AttendanceRuleID, opt => opt.MapFrom(s => s.AttendanceRuleID))
                .ForMember(s => s.RuleName, opt => opt.MapFrom(s => s.RuleName))
                .ForMember(s => s.Remark, opt => opt.MapFrom(s => s.Remark))
                .ForMember(s => s.EarlyLeaveRule, opt => opt.MapFrom(s => s.EarlyLeaveRule))
                .ForMember(s => s.LateRule, opt => opt.MapFrom(s => s.LateRule))
                .ForMember(s => s.NotClockRule, opt => opt.MapFrom(s => s.NotClockRule))
                .ReverseMap();
        }
    }
}
