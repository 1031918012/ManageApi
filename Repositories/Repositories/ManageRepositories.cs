using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{ 
    public class ManageRepositories:EFRepositories<ManageItem>, IManageReposotorie
    {
        public ManageRepositories(ManageContext context) : base(context) { }
    }
}
