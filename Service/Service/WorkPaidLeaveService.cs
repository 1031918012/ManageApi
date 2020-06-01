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
    public class WorkPaidLeaveService : IWorkPaidLeaveService
    {
        private readonly IAttendanceUnitOfWork _salaryUnitOfWork;
        public WorkPaidLeaveService(IAttendanceUnitOfWork salaryUnitOfWork)
        {
            _salaryUnitOfWork = salaryUnitOfWork;
        }

    }
}
