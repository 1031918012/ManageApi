﻿using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public interface ISalaryUnitOfWork:IUnitOfWork
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="TManage"></typeparam>
        /// <param name="manage"></param>
        void Add<TManage>(TManage manage) where TManage : class, IManage;
        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TManage"></typeparam>
        /// <param name="manage"></param>
        void Update<TManage>(TManage manage) where TManage : class, IManage;
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="TManage"></typeparam>
        /// <param name="manage"></param>
        void Delete<TManage>(TManage manage) where TManage : class, IManage;
    }
}
