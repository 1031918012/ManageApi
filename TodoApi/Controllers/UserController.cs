using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyEncrypt;
using Newtonsoft.Json;
using Service;
using System;
using System.Text.RegularExpressions;

namespace ManageApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]"), ApiExplorerSettings(GroupName = "SalaryCommon")]
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
        public IConfiguration Configuration { get; }
        /// <summary>
        /// 
        /// </summary>
        public RayPIToken rayPIToken { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userservice"></param>
        /// <param name="Configuration"></param>
        /// <param name="rayPIToken"></param>
        public UserController(IUserService userservice, IConfiguration Configuration, RayPIToken rayPIToken)
        {
            _userService = userservice;
            this.Configuration = Configuration;
            this.rayPIToken = rayPIToken;
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
        public string AddUser(string Uname, string password, string phone, string Icon, string UNickname, string Sub, string imageCheck)
        {
            if (Regex.IsMatch(phone, Configuration["Regex:Phone"]))
            {
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = false, Message = "请输入正确的手机号" });
            }
            if (_userService.GetUserRepeName(Uname))//判断重名
            {
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = false, Message = "用户名重复" });
            }
            if (_userService.GetUserRepePhone(phone))//一个手机号只能绑定一个用户
            {
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = false, Message = "一个用户只能绑定一个手机号" });
            }
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
            return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = true, Message = "添加用户成功" });
        }
        /// <summary>
        /// 登陆用户
        /// </summary>
        /// <param name="Uname">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost("Login")]
        public string Login(string Uname, string password)
        {
            if (string.IsNullOrEmpty(Uname))
            {
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = true, Message = "请输入正确的用户名" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = true, Message = "请输入密码" });
            }
            var user = _userService.GetUser(Uname);
            if (user == null)
            {
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = true, Message = "请输入正确的用户名" });
            }
            if (MD5Encrypt.Encrypt(password) != user.Password)
            {
                return JsonConvert.SerializeObject(new JsonResponse { IsSuccess = true, Message = "密码错误" });
            }
            string token = rayPIToken.IssueJWT(user, new TimeSpan(0, 1, 0), new TimeSpan(1, 0, 0));

            return "Bearer" + token;
        }
    }
}