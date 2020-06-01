using AutoMapper;
using Domain;
using Infrastructure;

namespace Service
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<Person, PersonDTO>()
                .ForMember(s => s.PersonID, opt => opt.MapFrom(s => s.PersonID))
                .ForMember(s => s.Department, opt => opt.MapFrom(s => s.Department))
                .ForMember(s => s.DepartmentID, opt => opt.MapFrom(s => s.DepartmentID))
                .ForMember(s => s.IDCard, opt => opt.MapFrom(s => s.IDCard))
                .ForMember(s => s.IDType, opt => opt.MapFrom(s => s.IDType))
                .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(s => s.IsBindWechat, opt => opt.MapFrom(s => (int)s.IsBindWechat))
                .ForMember(s => s.IsBindWechatConvert, opt => opt.MapFrom(s => s.IsBindWechat.GetDescription()))
                .ForMember(s => s.PhoneCode, opt => opt.MapFrom(s => s.PhoneCode))
                .ForMember(s => s.Position, opt => opt.MapFrom(s => s.Position))
                .ForMember(s => s.PositionID, opt => opt.MapFrom(s => s.PositionID))
                .ForMember(s => s.JobNumber, opt => opt.MapFrom(s => s.JobNumber))
                .ForMember(s => s.CompanyID, opt => opt.MapFrom(s => s.CompanyID))
                .ForMember(s => s.CustomerID, opt => opt.MapFrom(s => s.CustomerID))
                .ForMember(s => s.CompanyName, opt => opt.MapFrom(s => s.CompanyName))
                .ForMember(s => s.Id, opt => opt.MapFrom(s => s.PersonID))
                .ForMember(s => s.Hiredate, opt => opt.MapFrom(s => s.Hiredate))
                .ForMember(s => s.StartJobTime, opt => opt.MapFrom(s => s.StartJobTime))
                .ForMember(s => s.Sex, opt => opt.MapFrom(s => s.Sex))
                .ReverseMap();

            CreateMap<Person, PersonImportDTO>()
                .ForMember(s => s.Department, opt => opt.MapFrom(s => s.Department))
                .ForMember(s => s.DepartmentID, opt => opt.MapFrom(s => s.DepartmentID))
                .ForMember(s => s.IDCard, opt => opt.MapFrom(s => s.IDCard))
                .ForMember(s => s.IDType, opt => opt.MapFrom(s => s.IDType))
                .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(s => s.IsBindWechat, opt => opt.MapFrom(s => (int)s.IsBindWechat))
                .ForMember(s => s.IsBindWechatConvert, opt => opt.MapFrom(s => s.IsBindWechat.GetDescription()))
                .ForMember(s => s.PhoneCode, opt => opt.MapFrom(s => s.PhoneCode))
                .ForMember(s => s.Position, opt => opt.MapFrom(s => s.Position))
                .ForMember(s => s.JobNumber, opt => opt.MapFrom(s => s.JobNumber))
                .ForMember(s => s.CompanyID, opt => opt.MapFrom(s => s.CompanyID))
                .ForMember(s => s.CompanyName, opt => opt.MapFrom(s => s.CompanyName))
                .ForMember(s => s.Id, opt => opt.MapFrom(s => s.PersonID))
                .ForMember(s => s.Hiredate, opt => opt.MapFrom(s => s.Hiredate))
                .ForMember(s => s.StartJobTime, opt => opt.MapFrom(s => s.StartJobTime))
                .ForMember(s => s.Sex, opt => opt.MapFrom(s => s.Sex))
                .ReverseMap();
        }
    }
}
