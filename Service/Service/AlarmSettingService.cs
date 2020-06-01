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
    public class AlarmSettingService : IAlarmSettingService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IAlarmSettingRepository _alarmSettingRepository;
        private readonly IMapper _mapper;
        private readonly IAttendanceUnitOfWork _attendanceUnitOfWork;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alarmSettingRepository"></param>
        public AlarmSettingService(IAlarmSettingRepository alarmSettingRepository, IMapper mapper, IAttendanceUnitOfWork attendanceUnitOfWork)
        {
            _alarmSettingRepository = alarmSettingRepository;
            _mapper = mapper;
            _attendanceUnitOfWork = attendanceUnitOfWork;
        }

        /// <summary>
        /// 获取闹钟列表
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public List<AlarmSettingDTO> GetAlarmSettings(string IDCard)
        {
            List<AlarmSetting> list = _alarmSettingRepository.EntityQueryable<AlarmSetting>(s => s.IDCard == IDCard).ToList();
            return _mapper.Map<List<AlarmSetting>, List<AlarmSettingDTO>>(list);
        }

        /// <summary>
        /// 启用与禁用
        /// </summary>
        /// <param name="AlarmSettingID"></param>
        /// <returns></returns>
        public (bool, string) Enable(int AlarmSettingID)
        {
            var alarmSetting = _alarmSettingRepository.EntityQueryable<AlarmSetting>(s => s.AlarmSettingID == AlarmSettingID).FirstOrDefault();
            if (alarmSetting == null)
            {
                return (false, "该闹钟已不存在,刷新页面重试");
            }
            alarmSetting.IsEnable = !alarmSetting.IsEnable;
            _attendanceUnitOfWork.Update(alarmSetting);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "服务器内部错误");
            }
            return (true, (alarmSetting.IsEnable ? "启用" : "禁用") + "成功");
        }

        /// <summary>
        /// 新增闹钟
        /// </summary>
        /// <param name="IDCard"></param>
        /// <param name="hour"></param>
        /// <param name="minutes"></param>
        /// <param name="week"></param>
        /// <returns></returns>
        public (bool, string) AddAlarmSetting(string IDCard, int hour, int minutes, string week)
        {
            if (string.IsNullOrEmpty(week))
            {
                return (false, "请选择重复周期");
            }
            AlarmSetting alarmSetting = new AlarmSetting
            {
                Hour = hour,
                IDCard = IDCard,
                IsEnable = false,
                Minutes = minutes,
                Week = week
            };
            _attendanceUnitOfWork.Add(alarmSetting);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "服务器内部错误");
            }
            return (true, "新增成功");
        }

        /// <summary>
        /// 更改闹钟详情
        /// </summary>
        /// <param name="alarmSettingID"></param>
        /// <param name="hour"></param>
        /// <param name="minutes"></param>
        /// <param name="week"></param>
        /// <returns></returns>
        public (bool, string) UpdateAlarmSetting(int alarmSettingID, int hour, int minutes, string week)
        {
            if (string.IsNullOrEmpty(week))
            {
                return (false, "请选择重复周期");
            }
            var alarmSetting = _alarmSettingRepository.EntityQueryable<AlarmSetting>(s => s.AlarmSettingID == alarmSettingID).FirstOrDefault();
            if (alarmSetting == null)
            {
                return (false, "该闹钟已不存在,刷新页面重试");
            }
            alarmSetting.Hour = hour;
            alarmSetting.Minutes = minutes;
            alarmSetting.Week = week;
            _attendanceUnitOfWork.Update(alarmSetting);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "服务器内部错误");
            }
            return (true, "更改成功");
        }

        public (bool, string) DeleteAlarmSetting(int alarmSettingID)
        {
            var alarm = _alarmSettingRepository.EntityQueryable<AlarmSetting>(s=>s.AlarmSettingID== alarmSettingID).FirstOrDefault();
            if (alarm == null)
            {
                return (false, "该对象已删除刷新重试");
            }
            _attendanceUnitOfWork.Delete(alarm);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "服务器内部错误");
            }
            return (true, "删除成功");
        }
    }
}