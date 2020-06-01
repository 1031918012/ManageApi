using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class ClockInAddressProfile : Profile
    {
        public ClockInAddressProfile()
        {
            CreateMap<ClockInAddress, ClockInAddressDTO>()
                .ForMember(s => s.ClockName, opt => opt.MapFrom(s => s.ClockName))
                .ForMember(s => s.Distance, opt => opt.MapFrom(s => s.Distance))
                .ForMember(s => s.Latitude, opt => opt.MapFrom(s => s.Latitude))
                .ForMember(s => s.Longitude, opt => opt.MapFrom(s => s.Longitude))
                .ForMember(s => s.LatitudeBD, opt => opt.MapFrom(s => s.LatitudeBD))
                .ForMember(s => s.LongitudeBD, opt => opt.MapFrom(s => s.LongitudeBD))
                .ForMember(s => s.SiteName, opt => opt.MapFrom(s => s.SiteName))
                .ForMember(s => s.ClockInAddressID, opt => opt.MapFrom(s => s.ClockInAddressID))
                .ReverseMap();

            CreateMap<ClockInAddress, ClockInAddressAddDTO>()
               .ForMember(s => s.ClockName, opt => opt.MapFrom(s => s.ClockName))
               .ForMember(s => s.Distance, opt => opt.MapFrom(s => s.Distance))
               .ForMember(s => s.Latitude, opt => opt.MapFrom(s => s.Latitude))
               .ForMember(s => s.Longitude, opt => opt.MapFrom(s => s.Longitude))
               .ForMember(s => s.LatitudeBD, opt => opt.MapFrom(s => s.LatitudeBD))
               .ForMember(s => s.LongitudeBD, opt => opt.MapFrom(s => s.LongitudeBD))
               .ForMember(s => s.SiteName, opt => opt.MapFrom(s => s.SiteName))
               .ReverseMap();
        }
    }

}
