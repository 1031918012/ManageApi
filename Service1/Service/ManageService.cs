using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class ManageService: BaseService, IManageService
    {
        private readonly IManageRepository _manage;
        public ManageService(ISalaryUnitOfWork salaryUnitOfWork, IManageRepository manage) : base(salaryUnitOfWork)
        {
            _manage = manage;
        }

        public bool Add(ManageItem manage)
        {
            _salaryUnitOfWork.Add(manage);
           return _salaryUnitOfWork.Commit();
        }

        public bool Update(ManageItem manage)
        {
            _salaryUnitOfWork.Update(manage);
            return _salaryUnitOfWork.Commit();
        }
        public ManageItem SelectEntity(Guid id)
        {
           return _manage.MyCompileQuerySingle(s => s.BookID == id);
        }
        public List<ManageItem> SelectList()
        {
            return _manage.MyCompileQuery(s => s.BookID != null).ToList();
        }
    }
}
