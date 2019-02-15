using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repositories
{
    public class EFRepositories<TManage>: IRepositories<TManage> where TManage : class, IManage
    {
        public ManageContext _context;
        public EFRepositories(ManageContext context)
        {
            _context = context;
        }

        public IEnumerable<TManage> GetEntitieList(Expression<Func<TManage, bool>> exp)
        {
            return CompileQuery(exp);
        }

        public IEnumerable<TManage> GetEntitiesForPaging(int pageIndex, int pageSize, Expression<Func<TManage, bool>> exp)
        {
            var list = CompileQuery(exp).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            int totalNumber = list.Count();
            return new PageResult<TManage>(totalNumber, (totalNumber + pageSize - 1) / pageSize, pageIndex, pageSize, list);

        }

        public TManage GetEntity(Expression<Func<TManage, bool>> exp)
        {
            return CompileQuerySingle(exp);
        }
        /// <summary>
        /// 查列表 //有排序
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private IEnumerable<TManage> CompileQuery(Expression<Func<TManage, bool>> exp)
        {
            //排序处理

            //查内容
            var func = EF.CompileQuery((DbContext context) => context.Set<TManage>().Where(exp));
            return func(_context);
        }
        /// <summary>
        /// 查实体 //无排序
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private TManage CompileQuerySingle(Expression<Func<TManage, bool>> exp)
        {
            var func = EF.CompileQuery((DbContext context) => context.Set<TManage>().FirstOrDefault(exp));
            return func(_context);
        }
    }
}
