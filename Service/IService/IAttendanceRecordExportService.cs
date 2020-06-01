using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IAttendanceRecordExportService : IService
    {
        byte[] ExportRepairExcel();
        byte[] ExportLeaveExcel();
        byte[] ExportOutExcel();
        byte[] ExportOvertimeSt(FyuUser user, DateTime time); 
        byte[] ExportMissingClockSt(FyuUser user, DateTime time);
        byte[] ExportLateSt(FyuUser user, DateTime time);
        byte[] ExportWorkSt(FyuUser user, DateTime time);
    }
}
