using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IOvertimeService : IService
    {
        PageResult<OvertimePageDTO> OvertimePage(int pageIndex, int pageSize, FyuUser user);
        (bool, string) InsertOvertime(OvertimeDTO overtimeDTO, FyuUser user);
        (bool, string) UpdateOvertime(OvertimeDTO overtimeDTO, FyuUser user);
        (bool, string) DeleteOvertime(string overtimeID);
        OvertimeDTO GetOvertime(string overtimeID);
        List<OvertimeGroupDTO> OvertimeList(FyuUser user);
    }
}
