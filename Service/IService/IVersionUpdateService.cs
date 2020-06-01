using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IVersionUpdateService : IService
    {
        /// <summary>
        /// 分页查询版本更新
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PageResult<VersionUpdateDTO> GetVersionUpdatePage(int pageIndex, int pageSize);
        /// <summary>
        /// 新增版本更新
        /// </summary>
        /// <param name="updateTime"></param>
        /// <param name="versionNumber"></param>
        /// <param name="versionTitle"></param>
        /// <param name="versionContent"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        (bool, string) AddVersionUpdate(DateTime updateTime, string versionNumber, string versionTitle, List<string> versionContent, FyuUser user);
        /// <summary>
        /// 修改版本更新
        /// </summary>
        /// <param name="versionUpdateID"></param>
        /// <param name="updateTime"></param>
        /// <param name="versionNumber"></param>
        /// <param name="versionTitle"></param>
        /// <param name="versionContent"></param>
        /// <returns></returns>
        (bool, string) UpdateVersionUpdate(int versionUpdateID, DateTime updateTime, string versionNumber, string versionTitle, List<string> versionContent);
        /// <summary>
        /// 获取版本更新实体
        /// </summary>
        /// <param name="versionUpdateID"></param>
        /// <returns></returns>
        VersionUpdateDTO GetVersionUpdate(int versionUpdateID);
    }
}
