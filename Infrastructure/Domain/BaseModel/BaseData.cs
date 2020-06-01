namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseData
    {
        /// <summary>
        /// 
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IDCardSortNumber { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 姓名对应的序号 
        /// </summary>
        public int NameSortNumber { get; set; }
    }
}
