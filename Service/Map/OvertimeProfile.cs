using AutoMapper;
using Domain;
using Infrastructure;

namespace Service
{
    public class OvertimeProfile : Profile
    {
        public OvertimeProfile()
        {
            CreateMap<Overtime, OvertimePageDTO>()
                .ForMember(s => s.OverTimeID, opt => opt.MapFrom(s => s.OvertimeID))
                .ForMember(s => s.OverTimeName, opt => opt.MapFrom(s => s.OvertimeName))
                .ForMember(s => s.WorkingRule, opt => opt.MapFrom(s => "工作日：" +  s.WorkingCalculationMethod.GetDescription()))
                .ForMember(s => s.RestRule, opt => opt.MapFrom(s => "休息日：" + s.RestCalculationMethod.GetDescription()))
                .ForMember(s => s.HolidayRule, opt => opt.MapFrom(s => "节假日：" + s.HolidayCalculationMethod.GetDescription()))
                .ForMember(s => s.WorkingCompensation, opt => opt.MapFrom(s => "工作日：" + s.WorkingCompensationMode.GetDescription()))
                .ForMember(s => s.RestCompensation, opt => opt.MapFrom(s => "休息日：" + s.RestCompensationMode.GetDescription()))
                .ForMember(s => s.HolidayCompensation, opt => opt.MapFrom(s => "节假日：" + s.HolidayCompensationMode.GetDescription()))
                .ReverseMap();

            CreateMap<Overtime, OvertimeDTO>()
               .ForMember(s => s.OverTimeID, opt => opt.MapFrom(s => s.OvertimeID))
               .ForMember(s => s.OverTimeName, opt => opt.MapFrom(s => s.OvertimeName))
               .ForMember(s => s.WorkingCalculationMethod, opt => opt.MapFrom(s => s.WorkingCalculationMethod))
               .ForMember(s => s.WorkingCompensationMode, opt => opt.MapFrom(s => s.WorkingCompensationMode))
               .ForMember(s => s.RestCalculationMethod, opt => opt.MapFrom(s => s.RestCalculationMethod))
               .ForMember(s => s.RestCompensationMode, opt => opt.MapFrom(s => s.RestCompensationMode))
               .ForMember(s => s.HolidayCompensationMode, opt => opt.MapFrom(s => s.HolidayCompensationMode))
               .ForMember(s => s.HolidayCalculationMethod, opt => opt.MapFrom(s => s.HolidayCalculationMethod))
               .ForMember(s => s.LongestOvertime, opt => opt.MapFrom(s => s.LongestOvertime))
               .ForMember(s => s.MinimumOvertime, opt => opt.MapFrom(s => s.MinimumOvertime))
               .ForMember(s => s.ExcludingOvertime, opt => opt.MapFrom(s => s.ExcludingOvertime))
               .ReverseMap();
        }
    }
}
