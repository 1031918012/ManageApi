using Domain;
using Infrastructure;
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
        public AttendanceUnitOfWork(DBContextBase dBContextBase, ILogger<AttendanceUnitOfWork> logger)
        {
            ContextBase = dBContextBase;
            _logger = logger;
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
        #endregion

        public bool Commit()
        {
            using (var tran = ContextBase.Database.BeginTransaction())
            {
                try
                {
                    ContextBase.SaveChanges();
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    tran.Rollback();
                    return false;
                }
                finally
                {
                    tran.Dispose();
                }
            }

        }
    }
}
