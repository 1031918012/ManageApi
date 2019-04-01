using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Domain;

namespace Service
{
    public interface IUserService
    {
        /// <summary>
        /// 注册添加用户
        /// </summary>
        /// <param name="a">外层拼出完成模型</param>
        /// <returns></returns>
        bool AddUser(User a);
        /// <summary>
        /// 判断是否重名(账号)
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        bool GetUserRepeName(string uname);
        /// <summary>
        /// 判断是否重复手机号
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        bool GetUserRepePhone(string uname);
        /// <summary>
        /// 获取用户账号信息
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        User GetUser(string uname);
    }
}
