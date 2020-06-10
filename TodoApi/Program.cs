using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

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
            //.UseConfiguration(new ConfigurationBuilder().AddJsonFile("").Build())包含配置文件  能够使用一个静态类存储所有配置信息

            .UseUrls("http://*:5001;http://*:5002")  //.Start()能起到相同的作用  它将监听指定的 URL;
                                                     //.UseKestrel()//指定程序集会使用Kestrel服务器   必须同时指定UseKestrel和UseIISIntegration。Kestrel被设计为在代理后运行而不应该直接部署到互联网。UseIISIntegration指定IIS为反向代理服务器。
                                                     //.UseIISIntegration() //有此项制定IIs为启动环境
            .CaptureStartupErrors(true)////捕捉startup管道中的异常
            .UseContentRoot(Directory.GetCurrentDirectory())//应用程序目录，，为当前文件夹目录
            .UseWebRoot(Directory.GetCurrentDirectory() + "wwwroot")//静态文件目录，存在wwwroot下。。如果不指定，默认是 (Content Root Path)\wwwroot
            .UseEnvironment("Development")      //环境变量                                             
            .ConfigureLogging((context, loggingBuilder) =>
            {
                loggingBuilder.AddFilter("System", LogLevel.Warning);
                loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);//过滤掉系统默认的一些日志
                loggingBuilder.AddLog4Net();
            })
            .UseSetting("applicationName", "MyHost")//设置应用程序服务名称
            .UseSetting("detailedErrors", "true")//指定程序应用程序会显示详细的启动错误信息
            .UseStartup<Startup>()//管道   如果调用多次 UseStartup 方法，最后一个调用的生效。
                                  //设置中间件
            //.Configure(app =>
            //     {
            //         app.Run(async (y) => await y.Response.WriteAsync("Welcome to My App"));
            //     })
            .Build();

    }
}
