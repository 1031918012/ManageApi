using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class ShiftManagementProfile : Profile
    {
        public ShiftManagementProfile()
        {
            CreateMap<ShiftManagement, ShiftManagementDTO>()
                .ForMember(s => s.ShiftID, opt => opt.MapFrom(s => s.ShiftID))
                .ForMember(s => s.ShiftName, opt => opt.MapFrom(s => s.ShiftName))
                .ForMember(s => s.AttendanceTime, opt => opt.MapFrom(s => s.AttendanceTime))
                .ForMember(s => s.WorkHours, opt => opt.MapFrom(s => s.WorkHours))
                .ForMember(s => s.ShiftRemark, opt => opt.MapFrom(s => s.ShiftRemark))
                .ForMember(s => s.ClockRule, opt => opt.MapFrom(s => s.ClockRule.GetDescription()))
                .ForMember(s => s.ClockRuleType, opt => opt.MapFrom(s => (int)s.ClockRule))
                .ForMember(s => s.CompanyID, opt => opt.MapFrom(s => s.CompanyID))
                .ForMember(s => s.CompanyName, opt => opt.MapFrom(s => s.CompanyName))
                .ForMember(s => s.CreateTime, opt => opt.MapFrom(s => s.CreateTime))
                .ForMember(s => s.IsExemption, opt => opt.MapFrom(s => s.IsExemption))
                .ForMember(s => s.LateMinutes, opt => opt.MapFrom(s => s.LateMinutes))
                .ForMember(s => s.EarlyLeaveMinutes, opt => opt.MapFrom(s => s.EarlyLeaveMinutes))
                .ForMember(s => s.IsFlexible, opt => opt.MapFrom(s => s.IsFlexible))
                .ForMember(s => s.FlexibleMinutes, opt => opt.MapFrom(s => s.FlexibleMinutes));

            CreateMap<ShiftManagementDTO, ShiftManagement>()
                .ForMember(s => s.ShiftID, opt => opt.MapFrom(s => s.ShiftID))
                .ForMember(s => s.ShiftName, opt => opt.MapFrom(s => s.ShiftName))
                .ForMember(s => s.AttendanceTime, opt => opt.MapFrom(s => s.AttendanceTime))
                .ForMember(s => s.WorkHours, opt => opt.MapFrom(s => s.WorkHours))
                .ForMember(s => s.ShiftRemark, opt => opt.MapFrom(s => s.ShiftRemark))
                .ForMember(s => s.ClockRule, opt => opt.MapFrom(s => s.ClockRuleType))
                .ForMember(s => s.CompanyID, opt => opt.MapFrom(s => s.CompanyID))
                .ForMember(s => s.CompanyName, opt => opt.MapFrom(s => s.CompanyName))
                .ForMember(s => s.CreateTime, opt => opt.MapFrom(s => s.CreateTime))
                .ForMember(s => s.IsExemption, opt => opt.MapFrom(s => s.IsExemption))
                .ForMember(s => s.LateMinutes, opt => opt.MapFrom(s => s.LateMinutes))
                .ForMember(s => s.EarlyLeaveMinutes, opt => opt.MapFrom(s => s.EarlyLeaveMinutes))
                .ForMember(s => s.IsFlexible, opt => opt.MapFrom(s => s.IsFlexible))
                .ForMember(s => s.FlexibleMinutes, opt => opt.MapFrom(s => s.FlexibleMinutes));
        }
    }

    public static class ClockRuleEnumConvert
    {
        public static string ClockRuleEnumConvertTo(this ClockRuleEnum ClockRule)
        {
            string res = string.Empty;
            switch (ClockRule)
            {
                case ClockRuleEnum.SoonerOrLaterClock:
                    res = "早晚打卡";
                    break;
                case ClockRuleEnum.SegmentClock:
                    res = "分段打卡";
                    break;
            }
            return res;
        }
    }
}

