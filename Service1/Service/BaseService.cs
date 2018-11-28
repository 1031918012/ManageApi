using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class BaseService
    {
        public readonly ISalaryUnitOfWork _salaryUnitOfWork;

        public BaseService(ISalaryUnitOfWork salaryUnitOfWork)
        {
            _salaryUnitOfWork = salaryUnitOfWork;
        }
    }
}
