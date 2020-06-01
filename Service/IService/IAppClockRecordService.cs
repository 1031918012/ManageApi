using Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IAppClockRecordService : IService
    {

        (bool, WechatCodeEnum, string) ValidateBeforeClock(string IDCard);
        /// <summary>
        /// 获取是否跨天与打卡地点
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        AppClockInfoDTO GetClockInfo(string IDCard);
        /// <summary>
        /// 获取当日打卡记录
        /// </summary>
        /// <param name="IDCard"></param>
        /// <param name="isWorking"></param>
        /// <param name="isCross"></param>
        /// <returns></returns>
        List<ClockRecordLiteDTO> GetTodayClock(string IDCard);
        /// <summary>
        /// 判断内外勤打卡以及获取服务器时间
        /// </summary>
        /// <param name="IDCard"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="isCross"></param>
        /// <returns></returns>
        AppPostLocation PostLocation(string IDCard, double latitude, double longitude);
        /// <summary>
        /// 外勤打卡
        /// </summary>
        /// <param name="IDCard"></param>
        /// <param name="location"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="remark"></param>
        /// <param name="ImageBase64"></param>
        /// <param name="isCross"></param>
        /// <returns></returns>
        (bool, WechatCodeEnum, string) AddOutsideClock(string IDCard, string location, double latitude, double longitude, string remark, IFormFile ImageBase64, bool IsAnomaly);
        /// <summary>
        /// 内勤打卡
        /// </summary>
        /// <param name="IDCard"></param>
        /// <param name="location"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="isCross"></param>
        /// <returns></returns>
        (bool, WechatCodeEnum, string) AddInsideClock(string IDCard, string location, double latitude, double longitude, bool IsAnomaly);

        /// <summary>
        /// 获取日历
        /// </summary>
        /// <param name="Times"></param>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        List<AppMonthStatics> GetAppMonthStatics(DateTime Times, string IDCard);
        /// <summary>
        /// 获取月统计
        /// </summary>
        /// <param name="Times"></param>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        AppStatics GetAppStatics(DateTime Times, string IDCard);
        /// <summary>
        /// 获取当天记录统计
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        DayClockSelect GetAnyClock(DateTime dateTime, string IDCard);
        /// <summary>
        /// 获取某个月排班
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        List<AppShiftDTO> GetMonthShift(DateTime dateTime, string IDCard);
    }
}
