using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IAlarmSettingService : IService
    {
        /// <summary>
        /// 获取所有闹钟列表
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        List<AlarmSettingDTO> GetAlarmSettings(string IDCard);
        /// <summary>
        /// 闹钟的启用与关闭
        /// </summary>
        /// <param name="AlarmSettingID"></param>
        /// <returns></returns>
        (bool, string) Enable(int AlarmSettingID);
        /// <summary>
        /// 新增一个闹钟
        /// </summary>
        /// <param name="IDCard"></param>
        /// <param name="hour"></param>
        /// <param name="minutes"></param>
        /// <param name="week"></param>
        /// <returns></returns>
        (bool, string) AddAlarmSetting(string IDCard, int hour, int minutes, string week);
        /// <summary>
        /// 修改一个闹钟
        /// </summary>
        /// <param name="alarmSettingID"></param>
        /// <param name="hour"></param>
        /// <param name="minutes"></param>
        /// <param name="week"></param>
        /// <returns></returns>
        (bool, string) UpdateAlarmSetting(int alarmSettingID, int hour, int minutes, string week);
        (bool, string) DeleteAlarmSetting(int alarmSettingID);
    }
}
