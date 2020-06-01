using AutoMapper;
using Domain;
using Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Service
{
    public class AttendanceRecordProfile : Profile
    {
        public AttendanceRecordProfile()
        {
            CreateMap<AttendanceRecord, AttendanceRecordDTO>()
                .ForMember(s => s.AttendanceRecordID, opt => opt.MapFrom(s => s.AttendanceRecordID))
                .ForMember(s => s.IDCard, opt => opt.MapFrom(s => s.IDCard))
                .ForMember(s => s.CompanyID, opt => opt.MapFrom(s => s.CompanyID))
                .ForMember(s => s.CompanyName, opt => opt.MapFrom(s => s.CompanyName))
                .ForMember(s => s.AttendanceGroupName, opt => opt.MapFrom(s => s.AttendanceGroupName))
                .ForMember(s => s.AttendanceDate, opt => opt.MapFrom(s => s.AttendanceDate.ToString("yyyy-MM-dd") + s.AttendanceDate.DayOfWeek.ConvertWeek()))
                .ForMember(s => s.Shift, opt => opt.MapFrom(s => s.Shift))
                .ForMember(s => s.Status, opt => opt.MapFrom(s => s.Status.GetDescription()))
                .ForMember(s => s.Department, opt => opt.MapFrom(s => s.Department))
                .ForMember(s => s.DepartmentID, opt => opt.MapFrom(s => s.DepartmentID))
                .ForMember(s => s.CustomerID, opt => opt.MapFrom(s => s.CustomerID))
                .ForMember(s => s.EmployeeNo, opt => opt.MapFrom(s => s.EmployeeNo))
                .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(s => s.Position, opt => opt.MapFrom(s => s.Position))
                .ForMember(s => s.ShiftName, opt => opt.MapFrom(s => s.ShiftName))
                .ForMember(s => s.Position, opt => opt.MapFrom(s => s.Position))
                .ForMember(s => s.AttendanceItemDTOJson, opt => opt.MapFrom(s => JsonConvert.DeserializeObject<List<AttendanceItemForComputDTO>>(s.AttendanceItemDTOJson)))
                .ReverseMap();

            CreateMap<AttendanceRecord, ImportRecordDTO>()
               .ForMember(s => s.IDCard, opt => opt.MapFrom(s => s.IDCard))
               .ForMember(s => s.AttendanceDate, opt => opt.MapFrom(s => s.AttendanceDate.ToString("yyyy-MM-dd") + s.AttendanceDate.DayOfWeek.ConvertWeek()))
               .ForMember(s => s.Shift, opt => opt.MapFrom(s => s.Shift))
               .ForMember(s => s.Status, opt => opt.MapFrom(s => s.Status.GetDescription()))
               .ForMember(s => s.Department, opt => opt.MapFrom(s => s.Department))
               .ForMember(s => s.EmployeeNo, opt => opt.MapFrom(s => s.EmployeeNo))
               .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
               .ForMember(s => s.Position, opt => opt.MapFrom(s => s.Position))
               .ForMember(s => s.Position, opt => opt.MapFrom(s => s.Position))
               .ReverseMap();
        }
    }
    public static class ConvertShowTime
    {
        public static string ConvertTimeShow(this DateTime? time)
        {
            return time == new DateTime() ? "--" : string.Format("{0:t}", time);//"10:46";
        }
        public static string ConvertWeek(this DayOfWeek dayOfWeek)
        {
            string res = string.Empty;
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    res = "周日";
                    break;
                case DayOfWeek.Monday:
                    res = "周一";
                    break;
                case DayOfWeek.Tuesday:
                    res = "周二";
                    break;
                case DayOfWeek.Wednesday:
                    res = "周三";
                    break;
                case DayOfWeek.Thursday:
                    res = "周四";
                    break;
                case DayOfWeek.Friday:
                    res = "周五";
                    break;
                case DayOfWeek.Saturday:
                    res = "周六";
                    break;
            }
            return res;
        }
        public static string ConvertWeekStatic(this DayOfWeek dayOfWeek)
        {
            string res = string.Empty;
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    res = "星期日";
                    break;
                case DayOfWeek.Monday:
                    res = "星期一";
                    break;
                case DayOfWeek.Tuesday:
                    res = "星期二";
                    break;
                case DayOfWeek.Wednesday:
                    res = "星期三";
                    break;
                case DayOfWeek.Thursday:
                    res = "星期四";
                    break;
                case DayOfWeek.Friday:
                    res = "星期五";
                    break;
                case DayOfWeek.Saturday:
                    res = "星期六";
                    break;
            }
            return res;
        }
    }
}
