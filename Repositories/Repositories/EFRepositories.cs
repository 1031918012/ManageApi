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
        public IEnumerable<TManage> MyCompileQuery(Expression<Func<TManage, bool>> exp)
        {
            exp = exp ?? (s => 1 == 1);
            var func = EF.CompileQuery((DbContext context) => context.Set<TManage>().Where(exp));
            return func(_context);
        }
        public TManage MyCompileQuerySingle(Expression<Func<TManage, bool>> exp)
        {
            exp = exp ?? (s => 1 == 1);
            var func = EF.CompileQuery((DbContext context) => context.Set<TManage>().FirstOrDefault(exp));
            return func(_context);
        }
    }
}
