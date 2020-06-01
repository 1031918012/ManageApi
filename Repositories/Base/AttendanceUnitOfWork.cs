using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repositories
{
    public class AttendanceUnitOfWork : IAttendanceUnitOfWork
    {
        private DBContextBase ContextBase;
        private readonly ILogger<AttendanceUnitOfWork> _logger;
        private List<BatchModel> Sql;
        public AttendanceUnitOfWork(DBContextBase dBContextBase, ILogger<AttendanceUnitOfWork> logger)
        {
            ContextBase = dBContextBase;
            _logger = logger;
            Sql = new List<BatchModel>();
        }
        public Guid Id => new Guid();

        public void Add<T>(T entity) where T : class, IEntity
        {
            ContextBase.Add(entity);
        }

        /// <summary>
        /// 修改单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Update<T>(T entity) where T : class, IEntity
        {
            ContextBase.Update(entity);
        }
        /// <summary>
        /// 修改所有实体（待修改）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        public void Update<T>(List<T> models) where T : class, IEntity
        {
            ContextBase.UpdateRange(models);
        }
        /// <summary>
        /// 删除单个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Delete<T>(T entity) where T : class, IEntity
        {
            ContextBase.Remove(entity);
        }

        #region 批量操作
        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        public void BatchDelete<T>(Expression<Func<T, bool>> exp) where T : class, new()
        {
            exp = exp ?? (s => 1 == 1);
            var query = ContextBase.Set<T>().Where(exp).AsNoTracking();
            var sql = Batch.GetSqlDelete(query);
            Sql.Add(new BatchModel(sql, null));
            //return count;
        }
        /// <summary>
        /// 批量修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        public void BatchUpdate<T>(Expression<Func<T, T>> updateExpression, Expression<Func<T, bool>> exp) where T : class, new()
        {
            exp = exp ?? (s => 1 == 1);
            var query = ContextBase.Set<T>().Where(exp).AsNoTracking();
            var (sql, sqlParameters) = Batch.GetSqlUpdate(query, updateExpression);

            Sql.Add(new BatchModel(sql, sqlParameters));

            //int count = ContextBase.Database.ExecuteSqlCommand(sql, sqlParameters);
            //return count;

        }
        /// <summary>
        /// 批量新增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void BatchInsert<T>(List<T> list) where T : class, new()
        {
            (string sql, List<MySqlParameter> parameter) = ContextBase.GetSqlInsert(list);
            Sql.Add(new BatchModel(sql, parameter));
            //int count = ContextBase.Database.ExecuteSqlCommand(sql);
            //return count;
        }
        ///// <summary>
        ///// 异步删除
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="exp"></param>
        ///// <returns></returns>
        //public void BatchDeleteAsync<T>(Expression<Func<T, bool>> exp) where T : class, new()
        //{
        //    exp = exp ?? (s => 1 == 1);
        //    var query = ContextBase.Set<T>().Where(exp).AsNoTracking();
        //    var sql = Batch.GetSqlDelete(query);
        //    Sql.Add(sql, null);
        //    //return await ContextBase.Database.ExecuteSqlCommandAsync(sql);
        //}
        ///// <summary>
        ///// 一部修改
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="updateExpression"></param>
        ///// <param name="exp"></param>
        ///// <returns></returns>
        //public void BatchUpdateAsync<T>(Expression<Func<T, T>> updateExpression, Expression<Func<T, bool>> exp) where T : class, new()
        //{
        //    exp = exp ?? (s => 1 == 1);
        //    var query = ContextBase.Set<T>().Where(exp).AsNoTracking();
        //    var (sql, sqlParameters) = Batch.GetSqlUpdate(query, updateExpression);
        //    Sql.Add(sql, sqlParameters);
        //    //return await ContextBase.Database.ExecuteSqlCommandAsync(sql, sqlParameters);
        //}
        ///// <summary>
        ///// 异步新增
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="list"></param>
        ///// <returns></returns>
        //public void BatchInsertAsync<T>(List<T> list) where T : class, new()
        //{
        //    var sql = ContextBase.GetSqlInsert(list);
        //    Sql.Add(sql, null);

        //    //var count = await ContextBase.Database.ExecuteSqlCommandAsync(sql);
        //    //return count;
        //}

        #endregion

        public bool Commit()
        {
            using (var tran = ContextBase.Database.BeginTransaction())
            {
                try
                {
                    ContextBase.SaveChanges();
                    for (int i = 0; i < Sql.Count; i++)
                    {
                        if (Sql[i].ParameterList == null || Sql[i].ParameterList.Count <= 0)
                        {
                            ContextBase.Database.ExecuteSqlCommand(Sql[i].Sql);
                        }
                        else
                        {
                            ContextBase.Database.ExecuteSqlCommand(Sql[i].Sql, Sql[i].ParameterList);
                        }
                    }
                    //foreach (var item in Sql)
                    //{
                    //    if (item.ParameterList == null || item.ParameterList.Count <= 0)
                    //    {
                    //        ContextBase.Database.ExecuteSqlCommand(item.Sql);
                    //    }
                    //    else
                    //    {
                    //        ContextBase.Database.ExecuteSqlCommand(item.Sql, item.ParameterList);
                    //    }
                    //}

                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    tran.Rollback();
                    Sql.Clear();
                    return false;
                }
                finally
                {
                    Sql.Clear();
                    tran.Dispose();
                }
            }

        }
    }
}
