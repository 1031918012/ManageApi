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

    }
}