using System;
using System.Security.Cryptography;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MyEncrypt;
using Newtonsoft.Json;
using Service;

namespace ManageApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]"),ApiExplorerSettings(GroupName = "SalaryCommon")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly IUserService _userService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userservice"></param>
        public UserController(IUserService userservice)
        {
            _userService = userservice;
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
        /// <param name="imageCheck">图片验证码</param>
        /// <returns></returns>
        [HttpPost("AddUser")]
        public string AddUser(string Uname, string password, string phone, string Icon, string UNickname, string Sub,string imageCheck)
        {
            //图像验证码
            var a = new User
            {
                ID = new Guid(),
                Password = MD5Encrypt.Encrypt(password),//加密
                Phone = phone,
                Sub = Sub,
                Uname = Uname,
                UNickname = UNickname
            };
            var result = _userService.AddUser(a);
            if (!result)
            {
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = false, Message = "添加用户失败" });
            }
            return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = true,Message = "添加用户成功" });
        }
    }
}