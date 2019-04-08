using Microsoft.Extensions.Configuration;
using System.IO;

namespace Infrastructure.Redis.Inite
{
    public class RedisConfigInfo
    {
        /// <summary>
        /// 可写的Redis链接地址
        /// format:ip1,ip2
        /// 
        /// 默认6379端口
        /// </summary>
        public string WriteServerList = "";
        /// <summary>
        /// 可读的Redis链接地址
        /// format:ip1,ip2
        /// </summary>
        public string ReadServerList = "";
        /// <summary>
        /// 最大写链接数
        /// </summary>
        public int MaxWritePoolSize = 60;
        /// <summary>
        /// 最大读链接数
        /// </summary>
        public int MaxReadPoolSize = 60;
        /// <summary>
        /// 本地缓存到期时间，单位:秒
        /// </summary>
        public int LocalCacheTime = 180;
        /// <summary>
        /// 自动重启
        /// </summary>
        public bool AutoStart = true;
        /// <summary>
        /// 是否记录日志,该设置仅用于排查redis运行时出现的问题,
        /// 如redis工作正常,请关闭该项
        /// </summary>
        public bool RecordeLog = false;

        public RedisConfigInfo()
        {
            //添加 json 文件路径
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            //创建配置根对象
            var configurationRoot = builder.Build();
            WriteServerList = configurationRoot.GetSection("RedisIp").GetSection("WriteServerList").Value;
            ReadServerList = configurationRoot.GetSection("RedisIp").GetSection("ReadServerList").Value;
        }
    }
}
