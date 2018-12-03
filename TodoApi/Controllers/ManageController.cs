using System;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service;
using Infrastructure;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using System.Web;

namespace TodoApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ManageController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IManageService _service;
        private IHostingEnvironment _hostingEnvironment;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="hostingEnvironment"></param>
        public ManageController(IManageService service, IHostingEnvironment hostingEnvironment)
        {
            _service = service;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="price">价格</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public string Add(string name,string price)
        {
            ManageItem manage = new ManageItem
            {
                ID = Guid.NewGuid(),
                Name = name,
                Price = price
            };
            var result = _service.Add(manage);
            return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = result, Message = "添加成功" });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("OutPut")]
        public FileResult OutPut()
        {
            byte[] a = new byte[1024 * 2];
            using (MemoryStream ms = new MemoryStream())
            {
                a = ms.GetBuffer();
                ms.Close();
            };
            return File(a, "text/plain", "测试.txt", true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Export")]
        public FileResult Export()
        {
            var data = _service.SelectList();
            var headers = new Dictionary<string, string>
                {
                    { "ID", "序号" }
                    ,{ "Name", "商品名称" }
                    ,{ "Price", "价格"}
                };
            var filename = HttpUtility.UrlEncode("测试能不能用中文.xlsx", Encoding.UTF8);
            var stream = ExcelHelper.ExportListToExcel(data, filename, headers);
            Response.Headers[HeaderNames.ContentDisposition] = new ContentDispositionHeaderValue("attachment") { FileName = filename }.ToString();
            return new FileStreamResult(stream, "application/ms-excel");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Export2")]
        public FileResult Export2()
        {
            var data = _service.SelectList();
            byte[] res = NPOIHelp.OutputExcel(data, list, task.TaskName + "错误数据表");
            return File(res, "application/ms-excel", "错误数据表.xlsx", true);
        }

    }
}
