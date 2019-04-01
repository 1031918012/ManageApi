using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class UserService : BaseService, IUserService
    {
        public readonly IUserRepository _userRepository;
        public UserService(ISalaryUnitOfWork salaryUnitOfWork, IUserRepository userRepository) : base(salaryUnitOfWork)
        {
            _userRepository = userRepository;
        }
        /// <summary>
        /// 注册添加用户
        /// </summary>
        /// <param name="a">外层拼出完成模型</param>
        /// <returns></returns>
        public bool AddUser(User a)
        {

            _salaryUnitOfWork.Add(a);
            return _salaryUnitOfWork.Commit();
        }
        /// <summary>
        /// 获取用户账号信息
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        public User GetUser(string uname)
        {
            return _userRepository.GetEntity(s => s.Uname == uname);
        }
        /// <summary>
        /// 判断是否重名(账号)
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        public bool GetUserRepeName(string uname)
        {
           return _userRepository.GetEntitieList(s => s.Uname == uname).Count() > 0;
        }
        /// <summary>
        /// 判断是否重复手机号
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        public bool GetUserRepePhone(string phone)
        {
           return _userRepository.GetEntitieList(s => s.Phone == phone).Count() > 0;
        }
    }
}
