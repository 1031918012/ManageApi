using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IBreakoffService : IService
    {
        PageResult<BreakoffPageDTO> GetBreakoffPage(int pageIndex, int pageSize, string Name, FyuUser user);
        PageResult<BreakoffDetailPageDTO> GetBreakoffDetailPage(string IDCard, int pageIndex, int pageSize, FyuUser user);
        (bool, string) UpdateBreakoff(string IDCard, int type, decimal ChangeTime, FyuUser user);
    }
}
