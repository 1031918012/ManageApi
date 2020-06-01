using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Repositories
{
    public class EFRepository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
    {
        public DBContextBase ContextBase;
        public EFRepository(DBContextBase dBContextBase)
        {
            ContextBase = dBContextBase;
        }

        #region Entity
        public TAggregateRoot GetEntity(Expression<Func<TAggregateRoot, bool>> exp = null)
        {
            return CompileQuerySingle(exp);
        }
        public IEnumerable<TAggregateRoot> GetEntityList(Expression<Func<TAggregateRoot, bool>> exp = null, Expression<Func<TAggregateRoot, object>> order = null, SortOrderEnum sortOrder = SortOrderEnum.UnSpecified)
        {
            IQueryable<TAggregateRoot> resQuery = CreateQuery(null, exp, order, sortOrder);
            return MyCompileQuery(resQuery);
        }
        public PageResult<TAggregateRoot> GetByPage(int pageIndex, int pageSize, Expression<Func<TAggregateRoot, bool>> exp = null, Expression<Func<TAggregateRoot, object>> order = null, SortOrderEnum sortOrder = SortOrderEnum.UnSpecified)
        {
            IQueryable<TAggregateRoot> resQuery = CreateQuery(null, exp, order, sortOrder);
            return GetByPage(pageIndex, pageSize, resQuery);
        }

        public PageResult<TAggregateRoot> GetByPage(int pageIndex, int pageSize, IQueryable<TAggregateRoot> query = null, Expression<Func<TAggregateRoot, bool>> exp = null, Expression<Func<TAggregateRoot, object>> order = null, SortOrderEnum sortOrder = SortOrderEnum.UnSpecified)
        {
            IQueryable<TAggregateRoot> resQuery = CreateQuery(query, exp, order, sortOrder);
            return GetByPage(pageIndex, pageSize, resQuery);
        }

        public PageResult<TAggregateRoot> GetByPage(int pageIndex, int pageSize, IQueryable<TAggregateRoot> query = null, Expression<Func<TAggregateRoot, bool>> exp = null, Dictionary<Expression<Func<TAggregateRoot, object>>, SortOrderEnum> order = null)
        {
            IQueryable<TAggregateRoot> resQuery = CreateQuery(query, exp, order);
            return GetByPage(pageIndex, pageSize, resQuery);
        }
        #endregion

        #region IQueryable<T>
        public IQueryable<T> EntityQueryable<T>(Expression<Func<T, bool>> exp = null, bool isNoTracking = false) where T : class, IEntity
        {

            //ContextBase.Database.
            return isNoTracking ? ContextBase.Set<T>().Where(exp).AsNoTracking() : ContextBase.Set<T>().Where(exp);
        }



        public PageResult<T> GetByPage<T>(int pageIndex, int pageSize, IQueryable<T> query = null) where T : class, IEntity
        {
            int totalNumber = query.Count();
            List<T> list = MyCompileQuery(query.Skip((pageIndex - 1) * pageSize).Take(pageSize)).ToList();
            return new PageResult<T>(totalNumber, (totalNumber + pageSize - 1) / pageSize, pageIndex, pageSize, list);
        }
        #endregion



        private TAggregateRoot CompileQuerySingle(Expression<Func<TAggregateRoot, bool>> exp)
        {
            exp = exp ?? (s => 1 == 1);
            var func = EF.CompileQuery((DbContext context) => context.Set<TAggregateRoot>().FirstOrDefault(exp));
            return func(ContextBase);
        }
        private IQueryable<TAggregateRoot> CreateQuery(IQueryable<TAggregateRoot> query, Expression<Func<TAggregateRoot, bool>> exp, Expression<Func<TAggregateRoot, object>> order, SortOrderEnum sortOrder)
        {
            exp = exp ?? (s => 1 == 1);
            query = query == null ? ContextBase.Set<TAggregateRoot>().Where(exp) : query.Where(exp);
            if (order != null)
            {
                switch (sortOrder)
                {
                    case SortOrderEnum.Ascending:
                        query = query.OrderBy(order);
                        break;
                    case SortOrderEnum.Descending:
                        query = query.OrderByDescending(order);
                        break;
                    default:
                        break;
                }
            }
            return query;
        }
        private IQueryable<TAggregateRoot> CreateQuery(IQueryable<TAggregateRoot> query, Expression<Func<TAggregateRoot, bool>> exp, Dictionary<Expression<Func<TAggregateRoot, object>>, SortOrderEnum> order)
        {
            exp = exp ?? (s => 1 == 1);
            query = query == null ? ContextBase.Set<TAggregateRoot>().Where(exp) : query.Where(exp);
            if (order != null)
            {

                foreach (var item in order)
                {
                    switch (item.Value)
                    {
                        case SortOrderEnum.Ascending:
                            query = query.OrderBy(item.Key);
                            break;
                        case SortOrderEnum.Descending:
                            query = query.OrderByDescending(item.Key);
                            break;
                        default:
                            break;
                    }
                }
            }
            return query;
        }

        #region CompileQuery
        private IEnumerable<T> MyCompileQuery<T>(IQueryable<T> query)
        {
            var func = EF.CompileQuery((DbContext context) => query);
            return func(ContextBase);
        }
        #endregion
    }
}
