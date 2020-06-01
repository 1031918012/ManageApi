using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Service
{
    public static class LocationUtil
    {
        //地球半径，单位：米
        //适用于WGS-84及其衍生的坐标系
        private const double EARTH_RADIUS = 6378137;
        private const string AMAP_KEY = "746eab333f9db749015a738e3d4eff4d";
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// 计算两点位置的距离，返回两点的距离，单位：米
        /// 该公式为Google提供，误差小于0.2米
        /// </summary>
        /// <param name="lng1">第一点经度</param>
        /// <param name="lat1">第一点纬度</param>        
        /// <param name="lng2">第二点经度</param>
        /// <param name="lat2">第二点纬度</param>
        /// <returns></returns>
        public static double GetDistance(double lng1, double lat1, double lng2, double lat2)
        {
            double radLat1 = Rad(lat1);
            double radLng1 = Rad(lng1);
            double radLat2 = Rad(lat2);
            double radLng2 = Rad(lng2);
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
            return result;
        }

        /// <summary>
        /// 经纬度转化成弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return d * Math.PI / 180d;
        }

        /// <summary>
        /// 百度坐标系转国测局坐标系
        /// </summary>
        /// <param name="latitudeBD"></param>
        /// <param name="longitudeBD"></param>
        /// <returns></returns>
        public static Point BD09toGCJ02(double latitudeBD, double longitudeBD)
        {
            var uri = $"https://restapi.amap.com/v3/assistant/coordinate/convert?locations={longitudeBD.ToString("F6")},{latitudeBD.ToString("F6")}&coordsys=baidu&key={AMAP_KEY}";
            var task = Task.Run(() => httpClient.GetStringAsync(uri));
            task.Wait();
            var response = task.Result;
            var result = JObject.Parse(response);
            if (result["status"].ToString() == "1")
            {
                string[] array = result["locations"].ToString().Split(',');
                double lng = Convert.ToDouble(array[0]);
                double lat = Convert.ToDouble(array[1]);
                return new Point { Latitude = lat, Longitude = lng };
            }
            else
                return null;
        }
    }

    public class Point
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
    }
}
