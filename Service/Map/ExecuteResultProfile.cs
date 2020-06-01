using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class ExecuteResultProfile : Profile
    {
        public ExecuteResultProfile()
        {
            CreateMap<ExecuteResult, ExecuteResult2DTO>()
                .ForMember(s => s.Id, opt => opt.MapFrom(s => s.id))
                .ForMember(s => s.Name, opt => opt.MapFrom(s => s.label))
                .ForMember(s => s.Children, opt => opt.MapFrom(s => s.children))
                .ForMember(s => s.IsDepartment, opt => opt.MapFrom(s => true))
                .ReverseMap();

            CreateMap<PersonSelectDTO, ExecuteResult2DTO>()
                .ForMember(s => s.Id, opt => opt.MapFrom(s => s.DepartmentID))
                .ForMember(s => s.IDCard, opt => opt.MapFrom(s => s.IDCard))
                .ForMember(s => s.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(s => s.IsDepartment, opt => opt.MapFrom(s => false))
                .ReverseMap();

        }
    }
}
