using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class AttendanceRuleDetailProfile : Profile
    {
        public AttendanceRuleDetailProfile()
        {
            CreateMap<AttendanceRuleDetail, LateParam>()
                .ForMember(s => s.CallRuleType, opt => opt.MapFrom(s => s.CallRuleType))
                .ForMember(s => s.LateMaxJudge, opt => opt.MapFrom(s => s.MaxJudge))
                .ForMember(s => s.LateMaxTime, opt => opt.MapFrom(s => s.MaxTime))
                .ForMember(s => s.LateMinJudge, opt => opt.MapFrom(s => s.MinJudge))
                .ForMember(s => s.LateMinTime, opt => opt.MapFrom(s => s.MinTime))
                .ForMember(s => s.Time, opt => opt.MapFrom(s => s.Time))
                .ReverseMap();
            CreateMap<AttendanceRuleDetail, EarlyLeaveParam>()
                .ForMember(s => s.CallRuleType, opt => opt.MapFrom(s => s.CallRuleType))
                .ForMember(s => s.EarlyLeaveMaxJudge, opt => opt.MapFrom(s => s.MaxJudge))
                .ForMember(s => s.EarlyLeaveMaxTime, opt => opt.MapFrom(s => s.MaxTime))
                .ForMember(s => s.EarlyLeaveMinJudge, opt => opt.MapFrom(s => s.MinJudge))
                .ForMember(s => s.EarlyLeaveMinTime, opt => opt.MapFrom(s => s.MinTime))
                .ForMember(s => s.Time, opt => opt.MapFrom(s => s.Time))
                .ReverseMap();
            CreateMap<AttendanceRuleDetail, NotClockParam>()
                .ForMember(s => s.CallRuleType, opt => opt.MapFrom(s => s.CallRuleType))
                .ForMember(s => s.NotClockMaxJudge, opt => opt.MapFrom(s => s.MaxJudge))
                .ForMember(s => s.NotClockMaxTime, opt => opt.MapFrom(s => s.MaxTime))
                .ForMember(s => s.NotClockMinJudge, opt => opt.MapFrom(s => s.MinJudge))
                .ForMember(s => s.NotClockMinTime, opt => opt.MapFrom(s => s.MinTime))
                .ForMember(s => s.Time, opt => opt.MapFrom(s => s.Time))
                .ReverseMap();
        }
    }
}
