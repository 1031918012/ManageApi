using Domain;
using Infrastructure;
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
        List<TManage> GetEntitieList(Expression<Func<TManage, bool>> exp);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        PageResult<TManage> GetEntitiesForPaging(int pageIndex, int pageSize, Expression<Func<TManage, bool>> exp);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        TManage GetEntity(Expression<Func<TManage, bool>> exp);

    }
}