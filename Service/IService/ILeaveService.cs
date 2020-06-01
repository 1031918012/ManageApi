using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface ILeaveService : IService
    {
        PageResult<LeaveDTO> GetPageOut(int type, int pageIndex, int pageSize, FyuUser user);
    }
}
