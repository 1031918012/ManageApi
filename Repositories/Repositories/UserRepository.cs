using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{ 
    public class UserRepository:EFRepositories<User>, IUserRepository
    {
        public UserRepository(ManageContext context) : base(context) { }
    }
}
