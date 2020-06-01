using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure
{
    public static class ValidationManager
    {
        #region 身份证验证
        private static readonly int[] multiplyInts = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
        private static readonly string[] results = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
        //验证身份证号码
        public static bool IsChinaIDCard(string source)
        {
            source = source.ToLower();
            if (source.Length != 18) return false;
            if (!Regex.IsMatch(source, @"^\d{6}(18|19|20)\d{2}(0|1)\d(0|1|2|3)\d\d{3}(\d|X|x)$")) return false;
            string str17 = source.Substring(0, 17);
            if (!Regex.IsMatch(str17, @"[0-9]{17}")) return false;
            if (!Regex.IsMatch(str17,
                    @"^(11|12|13|14|15|21|22|23|31|32|33|34|35|36|37|41|42|43|44|45|46|50|51|52|53|54|61|62|63|64|65|71|81|82|91)"))
                return false;
            int year = Convert.ToInt32(source.Substring(6, 4));
            int month = Convert.ToInt32(source.Substring(10, 2));
            int day = Convert.ToInt32(source.Substring(12, 2));
            if (year > DateTime.Now.Year) return false;
            if (month > 12) return false;
            if (day > 31) return false;
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += multiplyInts[i] * Convert.ToInt32(str17[i].ToString());
            }
            sum = sum % 11;
            string res = results[sum];
            if (res.ToLower() != source.Substring(17, 1).ToLower()) return false;
            return true;
        }
        #endregion

        #region 其他奇奇怪怪的身份证
        #region 正则
        //9/11位
        private static readonly string HAReg = "[HMGhmg]{1}([0-9]{10}|[0-9]{8})";
        //9位
        private static readonly string PassPortReg = "[DESP]([0-9]{8})";
        //8位
        private static readonly string TaiPassReg = "[0-9]{8}";
        //15位
        private static readonly string ForeignIDCardReg = "[A-Za-z]{3}[0-9]{12}";

        #endregion
        public static bool IsPassPort(string source)
        {
            return Regex.IsMatch(source, PassPortReg);
        }
        public static bool IsHAPass(string source)
        {
            return Regex.IsMatch(source, HAReg);
        }
        public static bool IsTaiWanPass(string source)
        {
            return Regex.IsMatch(source, TaiPassReg);
        }
        public static bool IsForeignIDCard(string source)
        {
            return Regex.IsMatch(source, ForeignIDCardReg);
        }

        #endregion

        public static bool IsIDCard(string source)
        {
            switch (source.Length)
            {
                case 18:
                    return IsChinaIDCard(source);
                case 9:
                    var isHA = IsHAPass(source);
                    return isHA ? true : IsPassPort(source);
                case 11:
                    return IsHAPass(source);
                case 8:
                    return IsTaiWanPass(source);
                case 15:
                    return IsForeignIDCard(source);
                default:
                    return false;
            }
        }

        //private static readonly Dictionary<int, string> Province = new Dictionary<int, string> { { 11, "北京市" },{12,"天津市" },{13,"河北省" },{14,"山西省"},{15,"内蒙古自治区" },{21,"辽宁省" },{22,"吉林省" },{23,"黑龙江省" },{31,"上海市" },{32,"江苏省" },{33,"浙江省" },{34,"安徽省" },{35,"福建省" },{36,"江西省" },{37,"山东省" },{41,"河南省" },{42,"湖北省" },{43,"湖南省" },{44,"广东省" },{45,"广西壮族自治区" },{46,"海南省" },{50,"重庆市" },{51,"四川省" },{52,"贵州省" },{53,"云南省" },{54,"西藏自治区 "},{61,"陕西省" },{62,"甘肃省" },{63,"青海省" },{64,"宁夏回族自治区" },{65,"新疆维吾尔自治区" } };


        /// <summary>
        /// 提取证件信息
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public static IDInfoExtraction IDInfoExtract(string IDCard)
        {
            if (string.IsNullOrEmpty(IDCard))
            {
                return null;
            }
            IDInfoExtraction info = new IDInfoExtraction();
            switch (IDCard.Length)
            {
                //身份证
                case 18:
                    string strSex = string.Empty;
                    strSex = IDCard.Substring(14, 3);
                    info.Birth = IDCard.Substring(6, 4) + "-" + IDCard.Substring(10, 2) + "-" + IDCard.Substring(12, 2);
                    info.Age = CalculateAge(info.Birth);//根据生日计算年龄
                    if (int.TryParse(strSex, out int a))
                    {
                        info.Sex = a % 2 == 0 ? "女" : "男";
                    }
                    info.IDType = "居民身份证";
                    return info;
                case 9:
                    info.IDType = "中国护照";
                    //字母“D”开头的代表旧版无芯片的护照。字母“E”开头的代表有电子芯片的普通护照。字母“S”开头的代表公务护照。字母“P”开头的代表公务普通护照。
                    return info;
                case 11:
                    info.IDType = "港澳居民来往内地通行证";
                    //“H”字头签发给香港居民，“M”字头签发给澳门居民,前8位数字为通行证持有人的终身号(换证不变)，后2位数字表示换证次数，首次发证为00，此后依次递增。
                    return info;
                case 8:
                    info.IDType = "台湾居民来往大陆通行证";
                    //8位数字为通行证持有人的终身号(换证不变)
                    return info;
                case 15:
                    info.IDType = "外国护照";
                    return info;
                default:
                    return null;
            }
        }
        /// <summary>
        /// 计算年龄
        /// </summary>
        /// <param name="birthDay"></param>
        /// <returns></returns>
        public static int CalculateAge(string birthDay)
        {
            DateTime birthDate = DateTime.Parse(birthDay);
            DateTime nowDateTime = DateTime.Now;
            int age = nowDateTime.Year - birthDate.Year;
            //再考虑月、天的因素
            if (nowDateTime.Month < birthDate.Month || (nowDateTime.Month == birthDate.Month && nowDateTime.Day < birthDate.Day))
            {
                age--;
            }
            return age;
        }

    }
    /// <summary>
    /// 证件信息提取类
    /// </summary>
    public class IDInfoExtraction
    {
        public IDInfoExtraction(int Age = 0, string IDType = "", string Sex = "", string Birth = "", string Province = "", string City = "", string County = "")
        {
            this.IDType = IDType;
            this.Sex = Sex;
            this.Age = Age;
            this.Birth = Birth;
            this.Province = Province;
            this.City = City;
            this.County = County;
        }

        public string IDType { get; set; }//证件类型
        public string Sex { get; set; }//性别
        public int Age { get; set; }//年龄
        public string Birth { get; set; }//生日
        public string Province { get; set; }//户籍省
        public string City { get; set; }//户籍城市
        public string County { get; set; }//户籍县或区

    }
}
