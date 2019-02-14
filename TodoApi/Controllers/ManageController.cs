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
using System.Linq;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.AspNetCore.Http;

namespace ManageApi.Controllers
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
        private readonly IHostingEnvironment _hostingEnvironment;
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
        /// <param name="creator">创建人</param>
        /// <returns></returns>
        [HttpPost("Add")]
        public string Add(string name, string price, string creator)
        {
            try
            {
                ManageItem manage = new ManageItem
                {
                    BookID = Guid.NewGuid(),
                    Name = name,
                    Price = decimal.Parse(price),
                    CreateTime = DateTime.Now,
                    Creator = creator,
                    Isdelete = false,
                };
                var result = _service.Add(manage);
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = result, Message = "添加成功" });
        }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = false, Message = "价格输入有误" });
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ID">BOOKID</param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public string Delete(Guid ID)
        {
            var a = _service.SelectEntity(ID);
            if (a!=null)
            {
                a.Isdelete = true;
                var res = _service.Update(a);
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = res, Message = "删除成功" });
            }
            return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = false, Message = "该数据已经不存在，请刷新页面重试" });
        }
        /// <summary>
        /// 查询单个实例
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost("SelectEntity")]
        public string SelectEntity(Guid ID)
        {
            return JsonConvert.SerializeObject(_service.SelectEntity(ID));
        }
        /// <summary>
        /// 查询不分页列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("SelectList")]
        public string SelectList()
        {
            return JsonConvert.SerializeObject(_service.SelectList());
        }
        /// <summary>
        /// EPPlus简单例子
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
            return File(stream, "application/ms-excel");
        }
        /// <summary>
        /// NPOI简单例子
        /// </summary>
        /// <returns></returns>
        [HttpGet("Export2")]
        public FileResult Export2()
        {
            var data = _service.SelectList();
            byte[] res = NPOIHelp.OutputExcel(data, "错误数据表");
            return File(res, "application/ms-excel", "错误数据表.xlsx", true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("Import")]
        public string Import(IFormFile file)
        {
            var stream = file.OpenReadStream();
            try
            {
                var taskDataList = NPOIHelp.Import(stream);
                return null;
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = false, Message = "导入的数据格式有误!" });
            }

        }
    }
}
