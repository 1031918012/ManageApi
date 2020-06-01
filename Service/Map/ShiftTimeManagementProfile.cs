using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class ShiftTimeManagementProfile : Profile
    {
        public ShiftTimeManagementProfile()
        {
            CreateMap<ShiftTimeManagement, ShiftTimeManagementDTO>()
                .ForMember(s => s.ShiftTimeID, opt => opt.MapFrom(s => s.ShiftTimeID))
                .ForMember(s => s.ShiftID, opt => opt.MapFrom(s => s.ShiftID))
                .ForMember(s => s.ShiftName, opt => opt.MapFrom(s => s.ShiftName))
                .ForMember(s => s.ShiftTimeNumber, opt => opt.MapFrom(s => s.ShiftTimeNumber))
                .ForMember(s => s.StartWorkTime, opt => opt.MapFrom(s => s.StartWorkTime))
                .ForMember(s => s.EndWorkTime, opt => opt.MapFrom(s => s.EndWorkTime))
                .ForMember(s => s.StartRestTime, opt => opt.MapFrom(s => s.StartRestTime))
                .ForMember(s => s.EndRestTime, opt => opt.MapFrom(s => s.EndRestTime))
                .ForMember(s => s.UpStartClockTime, opt => opt.MapFrom(s => s.UpStartClockTime))
                .ForMember(s => s.UpEndClockTime, opt => opt.MapFrom(s => s.UpEndClockTime))
                .ForMember(s => s.DownStartClockTime, opt => opt.MapFrom(s => s.DownStartClockTime))
                .ForMember(s => s.DownEndClockTime, opt => opt.MapFrom(s => s.DownEndClockTime))
                .ReverseMap();
        }
    }
}

