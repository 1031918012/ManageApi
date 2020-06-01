using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public static class TimeConvert
    {
        public static DateTime ConvertTime(this DateTime dateTime)
        {
            return new DateTime(0001, 01, 01, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
        public static DateTime ConvertTime2(this DateTime dateTime)
        {
            return new DateTime(0001, 01, 01, dateTime.Hour, dateTime.Minute, 59);
        }
        public static DateTime ConvertTime3(this DateTime dateTime)
        {
            return new DateTime(0001, 01, 01, dateTime.Hour, dateTime.Minute, 0);
        }
        public static DateTime ConvertTime4(this DateTime dateTime)
        {
            return new DateTime(0001, 01, dateTime.Day, dateTime.Hour, dateTime.Minute/10*10, 0);
        }
        public static DateTime AddTime(this DateTime dateTime,DateTime time)
        {
            return dateTime.Date.AddDays(time.Day - 1).AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(time.Second);
        }
        public static DateTime ConvertTime5(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }
        public static DateTime ConvertTime6(this DateTime dateTime, DateTime time)
        {
            return new DateTime(dateTime.Year - time.Year+1, dateTime.Month - time.Month+1, dateTime.Day - time.Day+1, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
        public static DateTime ConvertTime7(this DateTime dateTime)
        {
            return new DateTime(0001, 01, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }
        /// <summary>
        /// 本地时间转成GMT格式的时间  
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToGMTFormat(this DateTime dt)
        {
            return dt.ToString("r") + dt.ToString("zzz").Replace(":", "");
        }
    }
}
