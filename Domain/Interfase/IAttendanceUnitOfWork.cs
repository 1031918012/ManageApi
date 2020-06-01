using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public interface IAttendanceUnitOfWork : IUnitOfWork, IBaseRepository
    {
        /// <summary>
        /// 上下文标识
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 新增单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Add<T>(T entity) where T : class, IEntity;

        /// <summary>
        /// 修改单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Update<T>(T entity) where T : class, IEntity;

        /// <summary>
        /// 修改所有实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        void Update<T>(List<T> models) where T : class, IEntity;

        /// <summary>
        /// 删除单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Delete<T>(T entity) where T : class, IEntity;

        #region 批量操作
        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        void BatchDelete<T>(Expression<Func<T, bool>> exp) where T : class, new();
        /// <summary>
        /// 用于条件修改状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="updateExpression"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        void BatchUpdate<T>(Expression<Func<T, T>> updateExpression, Expression<Func<T, bool>> exp) where T : class, new();
        /// <summary>
        /// 批量新增实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        void BatchInsert<T>(List<T> list) where T : class, new();
        ///// <summary>
        ///// 异步删除
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="exp"></param>
        ///// <returns></returns>
        //Task<int> BatchDeleteAsync<T>(Expression<Func<T, bool>> exp) where T : class, new();

        ///// <summary>
        ///// 一部修改
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="updateExpression"></param>
        ///// <param name="exp"></param>
        ///// <returns></returns>
        //Task<int> BatchUpdateAsync<T>(Expression<Func<T, T>> updateExpression, Expression<Func<T, bool>> exp) where T : class, new();
        ///// <summary>
        ///// 异步新增
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="list"></param>
        ///// <returns></returns>
        //Task<int> BatchInsertAsync<T>(List<T> list) where T : class, new();
        #endregion

    }
}
