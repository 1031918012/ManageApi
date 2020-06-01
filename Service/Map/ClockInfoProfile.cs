using AutoMapper;
using Domain;
using Infrastructure;
using System;

namespace Service
{
    public class ClockInfoProfile : Profile
    {
        public ClockInfoProfile()
        {
            CreateMap<ClockInfo, ClockInfoDTO>()
                .ForMember(s => s.ClockInfoID, opt => opt.MapFrom(s => s.ClockInfoID))
                .ForMember(s => s.AttendanceItemDTOJson, opt => opt.MapFrom(s => s.AttendanceItemDTOJson))
                .ForMember(s => s.AttendanceRecordID, opt => opt.MapFrom(s => s.AttendanceRecordID))
                .ForMember(s => s.ClockInResult, opt => opt.MapFrom(s => s.ClockInResult.GetDescription()))
                .ForMember(s => s.ClockInTime, opt => opt.MapFrom(s => s.ClockInTime.ConvertTimeShow()))
                .ForMember(s => s.ClockOutResult, opt => opt.MapFrom(s => s.ClockOutResult.GetDescription()))
                .ForMember(s => s.ClockOutTime, opt => opt.MapFrom(s => s.ClockOutTime.ConvertTimeShow()))
                .ForMember(s => s.EndLocation, opt => opt.MapFrom(s => s.EndLocation))
                .ForMember(s => s.StartLocation, opt => opt.MapFrom(s => s.StartLocation))
                .ForMember(s => s.ShiftTimeID, opt => opt.MapFrom(s => s.ShiftTimeID))
                .ReverseMap();
        }
    }
    public static class ClockInfoTime
    {
        public static string ConvertTimeShow(this DateTime time)
        {
            return time == new DateTime() ? "--" : string.Format("{0:t}", time);//"10:46";
        }
    }
}
