using System;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service;
using Infrastructure;

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public ManageController(IManageService service)
        {
            _service = service;
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
    }
}
