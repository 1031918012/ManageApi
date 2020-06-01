using AutoMapper;
using Domain;
using Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Service
{
    public class HolidayManagementProfile : Profile
    {
        public HolidayManagementProfile()
        {
            CreateMap<HolidayManagement, HolidayManagementPageDTO>()
                .ForMember(s => s.HolidayRule, opt => opt.MapFrom(s =>s.DateOfIssue.GetDescription()+s.QuotaRule.GetDescription()))
                .ForMember(s => s.IsEnable, opt => opt.MapFrom(s => s.EnableBalance?"开启余额":"未开启余额"))
                .ForMember(s => s.IsProhibit, opt => opt.MapFrom(s => s.IsProhibit?"启用":"未启用"))
                .ForMember(s => s.Name, opt => opt.MapFrom(s =>s.HolidayName ))
                .ForMember(s => s.HolidayManagementID, opt => opt.MapFrom(s =>s.HolidayManagementID ))
                .ReverseMap();

            CreateMap<HolidayManagement, HolidayParamDTO>()
              .ForMember(s => s.HolidayManagementID, opt => opt.MapFrom(s => s.HolidayManagementID))
              .ForMember(s => s.DateOfIssue, opt => opt.MapFrom(s => s.DateOfIssue))
              .ForMember(s => s.DistributionMethod, opt => opt.MapFrom(s => s.DistributionMethod))
              .ForMember(s => s.EnableBalance, opt => opt.MapFrom(s => s.EnableBalance))
              .ForMember(s => s.FixedData, opt => opt.MapFrom(s => s.FixedData))
              .ForMember(s => s.HolidayName, opt => opt.MapFrom(s => s.HolidayName))
              .ForMember(s => s.IssuingCycle, opt => opt.MapFrom(s => s.IssuingCycle))
              .ForMember(s => s.QuotaRule, opt => opt.MapFrom(s => s.QuotaRule))
              .ForMember(s => s.Seniority, opt => opt.MapFrom(s => JsonConvert.DeserializeObject<List<HolidayManagementItemDTO>>(s.Seniority)))
              .ForMember(s => s.WorkingYears, opt => opt.MapFrom(s => JsonConvert.DeserializeObject<List<HolidayManagementItemDTO>>(s.WorkingYears)))
              .ForMember(s => s.ValidityOfLimit, opt => opt.MapFrom(s => s.ValidityOfLimit))
              .ReverseMap();

        }
    }
}
