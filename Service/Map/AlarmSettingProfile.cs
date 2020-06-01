using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class AlarmSettingProfile : Profile
    {
        public AlarmSettingProfile()
        {
            CreateMap<AlarmSetting, AlarmSettingDTO>()
                .ForMember(s => s.AlarmSettingID, opt => opt.MapFrom(s => s.AlarmSettingID))
                .ForMember(s => s.Hour, opt => opt.MapFrom(s => s.Hour))
                .ForMember(s => s.IDCard, opt => opt.MapFrom(s => s.IDCard))
                .ForMember(s => s.Minutes, opt => opt.MapFrom(s => s.Minutes))
                .ForMember(s => s.Week, opt => opt.MapFrom(s => s.Week))
                .ForMember(s => s.IsEnable, opt => opt.MapFrom(s => s.IsEnable))
                .ReverseMap();
        }
    }
}
