using AutoMapper;
using Domain;
using Infrastructure;
using Newtonsoft.Json;
using System;

namespace Service
{
    public class PersonHolidayProfile : Profile
    {
        public PersonHolidayProfile()
        {
            CreateMap<Person, BalancePageDTO>()
               .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
               .ForMember(s => s.IDCard, opt => opt.MapFrom(s => s.IDCard))
               .ForMember(s => s.Department, opt => opt.MapFrom(s => s.Department))
               .ForMember(s => s.Seniority, opt => opt.MapFrom(s => DateTime.Now.Year - s.Hiredate.Year + ((decimal)(DateTime.Now.Month - s.Hiredate.Month) / 12)))
               .ForMember(s => s.WorkingYears, opt => opt.MapFrom(s => DateTime.Now.Year - s.StartJobTime.Year + ((decimal)(DateTime.Now.Month - s.StartJobTime.Month) / 12)))
               .ReverseMap();
        }
    }
}
