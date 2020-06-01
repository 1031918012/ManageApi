using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IAttendanceRecordMessageService : IService
    {
        void ClockMessage(DateTime time, List<ClockMessageDTO> clockMessage);

        /// <summary>
        /// 微信推送异常打卡信息
        /// </summary>
        /// <param name="time"></param>
        void ErrorRecordWechatMessage(DateTime time, List<ErrorMessageDTO> Records);
    }
}
