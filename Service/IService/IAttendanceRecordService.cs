using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IAttendanceRecordService : IService
    {
        /// <summary>
        /// 根据某时间计算当前时间与前一天
        /// </summary>
        /// <param name="time"></param>
        void DoAllComputeTwo(DateTime time);
        /// <summary>
        /// 计算某天所有打卡记录
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        (bool, string) DoAllComputeNew(DateTime time);
        /// <summary>
        /// 计算某个身份证的打卡记录
        /// </summary>
        /// <param name="IDcard"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        bool ComputeOneIDCard(string IDcard, DateTime time, bool clockType = true);
        bool DataUpdate();
        //void AutoBreakOff(DateTime time);
        bool DataUpdateOr(DateTime dateTime);
    }
}
