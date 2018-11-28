using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class EFRepositories<TManage>: IRepositories<TManage> where TManage : class, IManage
    {
        public ManageContext _context;
        public EFRepositories(ManageContext context)
        {
            _context = context;
        }
    }
}
