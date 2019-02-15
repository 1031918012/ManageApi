using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{ 
    public class ManageRepository:EFRepositories<ManageItem>, IManageRepository
    {
        public ManageRepository(ManageContext context) : base(context) { }
    }
}
