using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class AlarmSetting : IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int AlarmSettingID { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 闹钟小时
        /// </summary>
        public int Hour { get; set; }
        /// <summary>
        /// 闹钟分钟
        /// </summary>
        public int Minutes { get; set; }
        /// <summary>
        /// 重复周期
        /// </summary>
        public string Week { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public  bool IsEnable { get; set; }
    }
}
