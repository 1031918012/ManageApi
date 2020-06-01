using AutoMapper;
using Domain;
using Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Service
{
    public class AttendanceRuleDetailService : IAttendanceRuleDetailService
    {
        private readonly IAttendanceUnitOfWork _salaryUnitOfWork;
        public AttendanceRuleDetailService(IAttendanceUnitOfWork salaryUnitOfWork)
        {
            _salaryUnitOfWork = salaryUnitOfWork;
        }

    }
}
