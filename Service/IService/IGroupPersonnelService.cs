using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public interface IGroupPersonnelService : IService
    {
        FYWDataResult<string> GetAllActiveUsers(string customerId);
    }
}
