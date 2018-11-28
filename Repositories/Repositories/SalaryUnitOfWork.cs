﻿using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace Repositories
{
    class SalaryUnitOfWork : ISalaryUnitOfWork
    {
        private readonly ManageContext _context;

        public SalaryUnitOfWork(ManageContext context)
        {
            _context = context;
        }
        public bool Commit()
        {
            return _context.SaveChanges() > 0;
        }

        void ISalaryUnitOfWork.Add<TManage>(TManage manage)
        {
            _context.Set<TManage>().AddAsync(manage);

        }
    }
}
