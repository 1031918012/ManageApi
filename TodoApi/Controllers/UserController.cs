using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service;

namespace ManageApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]"), ApiExplorerSettings(GroupName = "user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IManageService _service;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public UserController(IManageService service)
        {
            _service = service;
        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="Uname">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="Icon">头像</param>
        /// <param name="UNickname">昵称</param>
        /// <param name="phone">电话</param>
        /// <param name="Sub">角色</param>
        /// <returns></returns>
        public string AddUser(string Uname,string password, string phone, string Icon, string UNickname, string Sub)
        {
            return JsonConvert.SerializeObject(new User());
        }
    }
}