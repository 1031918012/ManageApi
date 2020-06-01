using System;

namespace Domain
{
    /// <summary>
    /// 打卡地点表
    /// </summary>
    public class ClockInAddress : IAggregateRoot
    {
        /// <summary>
        /// 打卡地点表id
        /// </summary>
        public string ClockInAddressID { get; set; }
        /// <summary>
        /// 考勤地点(输入地址)
        /// </summary>
        public string SiteName { get; set; }
        /// <summary>
        /// 打卡名称(定位详细地址)
        /// </summary>
        public string ClockName { get; set; }
        /// <summary>
        /// 纬度（GCJ-02）
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 经度（GCJ-02）
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 纬度（BD-09）
        /// </summary>
        public double LatitudeBD { get; set; }
        /// <summary>
        /// 经度（BD-09）
        /// </summary>
        public double LongitudeBD { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public int Distance { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
