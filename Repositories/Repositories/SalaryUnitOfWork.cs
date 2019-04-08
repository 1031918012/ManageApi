using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Domain;
using EFCore.BulkExtensions;
using Infrastructure;

namespace Repositories
{
    public class SalaryUnitOfWork : ISalaryUnitOfWork
    {
        private readonly ManageContext _context;

        public SalaryUnitOfWork(ManageContext context)
        {
            _context = context;
        }

        public Guid Id => new Guid();
        public bool Commit()
        {
            return _context.SaveChanges() > 0;
        }

        void ISalaryUnitOfWork.Add<TManage>(TManage manage)
        {
            _context.Set<TManage>().AddAsync(manage);
        }

        void ISalaryUnitOfWork.Delete<TManage>(TManage manage)
        {
            _context.Set<TManage>().Remove(manage);
        }


        void ISalaryUnitOfWork.Update<TManage>(TManage manage)
        {
            _context.Set<TManage>().Update(manage);
        }
        
        void ISalaryUnitOfWork.AddRange<TManage>(List<TManage> manage)
        {
            _context.BulkInsert(manage);
        }

        void ISalaryUnitOfWork.AddRangeasyn<TManage>(List<TManage> manage)
        {
            _context.BulkInsertAsync(manage);
        }
    }
}
