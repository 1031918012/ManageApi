using Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Domain
{
    public interface IRepositories<TManage> where TManage : class,IManage
    {
        IEnumerable<TManage> MyCompileQuery(Expression<Func<TManage, bool>> exp);
        TManage MyCompileQuerySingle(Expression<Func<TManage, bool>> exp);
    }
}