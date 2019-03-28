using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Domain;

namespace Service
{
    public interface IUserService
    {
        bool AddUser(User a);
    }
}
