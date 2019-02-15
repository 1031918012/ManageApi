using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class PeopleRepository : EFRepositories<People>, IPeopleRepository
    {
        public PeopleRepository(ManageContext context) : base(context) { }
    }
}
