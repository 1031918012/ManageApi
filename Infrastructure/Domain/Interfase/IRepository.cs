using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Domain
{
    /// <summary>
    /// 聚合查询
    /// </summary>
    /// <typeparam name="TAggregateRoot"></typeparam>
    public interface IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot, IBaseRepository
    {
        /// <summary>
        /// 查询实体集合
        /// </summary>
        /// <param name="exp">查询条件</param>
        /// <returns></returns>
        TAggregateRoot GetEntity(Expression<Func<TAggregateRoot, bool>> exp = null);
        /// <summary>
        ///  查询聚合跟集合
        /// </summary>
        /// <param name="exp">查询条件</param>
        /// <param name="order">排序字段</param>
        /// <param name="sortOrder">排序条件</param>
        /// <returns>集合</returns>
        IEnumerable<TAggregateRoot> GetEntityList(Expression<Func<TAggregateRoot, bool>> exp = null, Expression<Func<TAggregateRoot, object>> order = null, SortOrderEnum sortOrder = SortOrderEnum.UnSpecified);
        /// <summary>
        /// 分页查询
        /// 单条件排序
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="exp">查询条件</param>
        /// <param name="order">排序字段</param>
        /// <param name="sortOrder">排序规则</param>
        /// <returns></returns>
        PageResult<TAggregateRoot> GetByPage(int pageIndex, int pageSize, Expression<Func<TAggregateRoot, bool>> exp = null, Expression<Func<TAggregateRoot, object>> order = null, SortOrderEnum sortOrder = SortOrderEnum.UnSpecified);
        /// <summary>
        /// 分页查询
        /// 单条件排序
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="query"></param>
        /// <param name="exp">查询条件</param>
        /// <param name="order">排序字段</param>
        /// <param name="sortOrder">排序规则</param>
        /// <returns></returns>
        PageResult<TAggregateRoot> GetByPage(int pageIndex, int pageSize, IQueryable<TAggregateRoot> query, Expression<Func<TAggregateRoot, bool>> exp = null, Expression<Func<TAggregateRoot, object>> order = null, SortOrderEnum sortOrder = SortOrderEnum.UnSpecified);
        /// <summary>
        /// 分页查询
        /// 多条件排序
        /// </summary>
        /// <param name="pageIndex">页面</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <param name="query"></param>
        /// <param name="exp">查询条件</param>
        /// <param name="order">排序字典(Key:排序字段  Value:排序规则)</param>
        /// <returns></returns>
        PageResult<TAggregateRoot> GetByPage(int pageIndex, int pageSize, IQueryable<TAggregateRoot> query, Expression<Func<TAggregateRoot, bool>> exp = null, Dictionary<Expression<Func<TAggregateRoot, object>>, SortOrderEnum> order = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="isNoTracking"></param>
        /// <returns></returns>
        IQueryable<T> EntityQueryable<T>(Expression<Func<T, bool>> exp = null, bool isNoTracking = false) where T : class, IEntity;
        /// <summary>
        /// 双查询分页(节约性能)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        PageResult<T> GetByPage<T>(int pageIndex, int pageSize, IQueryable<T> query = null) where T : class, IEntity;
    }
}
