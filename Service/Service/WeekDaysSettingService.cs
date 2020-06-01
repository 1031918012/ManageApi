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
    public class WeekDaysSettingService : IWeekDaysSettingService
    {
        private readonly IAttendanceUnitOfWork _salaryUnitOfWork;
        public WeekDaysSettingService(IAttendanceUnitOfWork salaryUnitOfWork)
        {
            _salaryUnitOfWork = salaryUnitOfWork;
        }

    }
}
