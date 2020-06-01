using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IAttendanceRecordAddService : IService
    {
        /// <summary>
        /// 添加某天所有打卡记录
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        (bool, string) DoAllAdd(DateTime time);
    }
}
