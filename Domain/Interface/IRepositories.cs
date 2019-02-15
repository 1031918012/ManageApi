using Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TManage"></typeparam>
    public interface IRepositories<TManage> where TManage : class,IManage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        IEnumerable<TManage> MyCompileQuery(Expression<Func<TManage, bool>> exp);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        TManage MyCompileQuerySingle(Expression<Func<TManage, bool>> exp);
    }
}