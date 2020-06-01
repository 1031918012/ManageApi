using AutoMapper;
using Domain;
using Infrastructure;
using Infrastructure.Cache;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Service
{
    public class VersionUpdateService : BaseService, IVersionUpdateService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IVersionUpdateRepository _versionUpdateRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alarmSettingRepository"></param>
        public VersionUpdateService(IVersionUpdateRepository versionUpdateRepository, IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer) : base(attendanceUnitOfWork, mapper, serializer)
        {
            _versionUpdateRepository = versionUpdateRepository;
        }

        /// <summary>
        /// 获取其版本更新分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public PageResult<VersionUpdateDTO> GetVersionUpdatePage(int pageIndex, int pageSize)
        {
            var versionUpdateQuery = _versionUpdateRepository.EntityQueryable<VersionUpdate>(s => true).OrderByDescending(s => s.UpdateTime);
            var versionUpdateList = _versionUpdateRepository.GetByPage(pageIndex, pageSize, versionUpdateQuery);
            return PageMap<VersionUpdateDTO, VersionUpdate>(versionUpdateList);
        }

        /// <summary>
        /// 新增版本更新方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public (bool, string) AddVersionUpdate(DateTime updateTime, string versionNumber, string versionTitle, List<string> versionContent, FyuUser user)
        {
            if (string.IsNullOrEmpty(versionNumber))
            {
                return (false, "请输入版本号");
            }
            if (string.IsNullOrEmpty(versionTitle))
            {
                return (false, "请输入版本标题");
            }
            if (!versionContent.Any())
            {
                return (false, "请输入版本内容");
            }
            VersionUpdate versionUpdate = new VersionUpdate
            {
                CreateTime = DateTime.Now,
                Creator = user.realName,
                CreatorID = user.userId,
                UpdateTime = updateTime,
                VersionNumber = versionNumber,
                VersionTitle = versionTitle,
                VersionContent = JsonConvert.SerializeObject(versionContent)
            };
            _attendanceUnitOfWork.Add(versionUpdate);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "服务器内部错误");
            }
            return (true, "添加成功");
        }
        /// <summary>
        /// 新增版本更新方法
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public (bool, string) UpdateVersionUpdate(int versionUpdateID, DateTime updateTime, string versionNumber, string versionTitle, List<string> versionContent)
        {
            if (string.IsNullOrEmpty(versionNumber))
            {
                return (false, "请输入版本号");
            }
            if (string.IsNullOrEmpty(versionTitle))
            {
                return (false, "请输入版本标题");
            }
            if (!versionContent.Any())
            {
                return (false, "请输入版本内容");
            }
            var versionUpdate = _versionUpdateRepository.EntityQueryable<VersionUpdate>(s => s.VersionUpdateID == versionUpdateID).FirstOrDefault();
            if (versionUpdate == null)
            {
                return (false, "该版本更新条目已不存在");
            }
            versionUpdate.UpdateTime = updateTime;
            versionUpdate.VersionNumber = versionNumber;
            versionUpdate.VersionTitle = versionTitle;
            versionUpdate.VersionContent = JsonConvert.SerializeObject(versionContent);
            _attendanceUnitOfWork.Update(versionUpdate);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "服务器内部错误");
            }
            return (true, "修改成功");
        }

        public VersionUpdateDTO GetVersionUpdate(int versionUpdateID)
        {
            var versionUpdate = _versionUpdateRepository.EntityQueryable<VersionUpdate>(s => s.VersionUpdateID == versionUpdateID).FirstOrDefault();
            if (versionUpdate == null)
            {
                return null;
            }
            return _mapper.Map<VersionUpdate, VersionUpdateDTO>(versionUpdate);
        }
    }
}