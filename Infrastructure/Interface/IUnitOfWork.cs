using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    /// <summary>
    /// 工作单元接口
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 提交 当前事务
        /// </summary>
        /// <returns></returns>
        bool Commit();
    }
}
