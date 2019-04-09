using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace TodoApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging((context, loggingBuilder) =>
            {
                loggingBuilder.AddFilter("System", LogLevel.Warning);
                loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);//过滤掉系统默认的一些日志
                loggingBuilder.AddLog4Net();
            })
                .Build();

    }
}
