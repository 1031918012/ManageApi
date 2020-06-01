using AutoMapper;
using Domain;
using Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class FeedbackProfile : Profile
    {
        public FeedbackProfile()
        {
            CreateMap<Feedback, FeedbackDTO>()
                .ForMember(s => s.FeedbackID, opt => opt.MapFrom(s => s.FeedbackID))
                .ForMember(s => s.FeedbackType, opt => opt.MapFrom(s => s.FeedbackType.GetDescription()))
                .ForMember(s => s.CreateTime, opt => opt.MapFrom(s => s.CreateTime.ToString("yyyy-MM-dd HH:mm")))
                .ForMember(s => s.Path, opt => opt.MapFrom(s => JsonConvert.DeserializeObject<List<string>>(s.Path)))
                .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(s => s.Content, opt => opt.MapFrom(s => s.Content))
                .ForMember(s => s.IDCard, opt => opt.MapFrom(s => s.IDCard))
                .ForMember(s => s.CompanyID, opt => opt.MapFrom(s => s.CompanyID))
                .ForMember(s => s.CompanyName, opt => opt.MapFrom(s => s.CompanyName))
                .ReverseMap();
        }
    }
}
