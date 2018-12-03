using Domain;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class ManageService: BaseService, IManageService
    {
        private readonly IManageReposotory _manage;
        public ManageService(ISalaryUnitOfWork salaryUnitOfWork, IManageReposotory manage) : base(salaryUnitOfWork)
        {
            _manage = manage;
        }

        public bool Add(ManageItem manage)
        {
            _salaryUnitOfWork.Add(manage);
           return _salaryUnitOfWork.Commit();
        }
        public List<ManageItem> SelectList()
        {
            return _manage.MyCompileQuery(s => s.ID != null).ToList();
        }
    }
}
