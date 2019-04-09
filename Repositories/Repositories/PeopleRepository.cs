using Domain;
using EFCore.BulkExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositories
{
    public class PeopleRepository : EFRepositories<People>, IPeopleRepository
    {
        public PeopleRepository(ManageContext context) : base(context) { }

        public void AAA()
        {
            _context.Users.Where(s => s.Phone == "123123").BatchDelete();


        }
    }
}
