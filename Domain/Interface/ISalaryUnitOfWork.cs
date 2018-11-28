using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public interface ISalaryUnitOfWork:IUnitOfWork
    {
        void Add<TManage>(TManage manage) where TManage : class, IManage;
    }
}
