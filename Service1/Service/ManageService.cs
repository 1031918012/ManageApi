using Domain;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class ManageService: BaseService, IManageService
    {
        private readonly IManageReposotorie _manage;
        public ManageService(ISalaryUnitOfWork salaryUnitOfWork, IManageReposotorie manage) : base(salaryUnitOfWork)
        {
            _manage = manage;
        }

        public bool Add(ManageItem manage)
        {
            _salaryUnitOfWork.Add(manage);
           return _salaryUnitOfWork.Commit();
        }
    }
}
