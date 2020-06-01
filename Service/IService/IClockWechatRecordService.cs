using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IClockWechatRecordService : IService
    {
        (bool, WechatCodeEnum) ValidateBeforeClock(string IDCard);
    }
}
