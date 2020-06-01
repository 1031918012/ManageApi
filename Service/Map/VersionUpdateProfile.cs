using AutoMapper;
using Domain;
using Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class VersionUpdateProfile : Profile
    {
        public VersionUpdateProfile()
        {
            CreateMap<VersionUpdate, VersionUpdateDTO>()
                .ForMember(s => s.UpdateTime, opt => opt.MapFrom(s => s.UpdateTime))
                .ForMember(s => s.VersionContent, opt => opt.MapFrom(s => JsonConvert.DeserializeObject<List<string>>(s.VersionContent)))
                .ForMember(s => s.VersionNumber, opt => opt.MapFrom(s => s.VersionNumber))
                .ForMember(s => s.VersionTitle, opt => opt.MapFrom(s => s.VersionTitle))
                .ForMember(s => s.VersionUpdateID, opt => opt.MapFrom(s => s.VersionUpdateID))
                .ReverseMap();
        }
    }
}
