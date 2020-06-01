using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Service
{
    public class EnterpriseSetService : IEnterpriseSetService
    {
        private readonly IMapper _mapper;
        private readonly IEnterpriseSetRepository _enterpriseSetRepository;
        private readonly IAttendanceItemCatagoryRepository _attendanceItemCatagoryRepository;
        private readonly IAttendanceUnitOfWork _salaryUnitOfWork;

        public EnterpriseSetService(IMapper mapper, IEnterpriseSetRepository enterpriseSetRepository, IAttendanceItemCatagoryRepository attendanceItemCatagoryRepository, IAttendanceUnitOfWork salaryUnitOfWork)
        {
            _mapper = mapper;
            _enterpriseSetRepository = enterpriseSetRepository;
            _attendanceItemCatagoryRepository = attendanceItemCatagoryRepository;
            _salaryUnitOfWork = salaryUnitOfWork;
        }

        /// <summary>
        /// 获取考勤项列表
        /// </summary>
        /// <returns></returns>
        public List<AttendanceItemCatagoryDTO> SelectAttendanceItem()
        {
            //查询
            var attendanceItemCatagoryList = _attendanceItemCatagoryRepository.GetEntityList().ToList();
            return _mapper.Map<List<AttendanceItemCatagoryDTO>>(attendanceItemCatagoryList);
        }

        /// <summary>
        /// 查询企业设置列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public PageResult<EnterpriseSetDTO> SelectEnterpriseSetList(int pageIndex, int pageSize, string companyID)
        {
            Expression<Func<EnterpriseSet, bool>> expression = s => true;
            if (!string.IsNullOrEmpty(companyID))
            {
                expression = expression.And(s => companyID.Contains(s.CompanyID));
            }
            //查询
            var enterpriseSet = _enterpriseSetRepository.EntityQueryable(expression).Select(s => new EnterpriseSetDTO { CompanyID = s.CompanyID, CompanyName = s.CompanyName, CreateTime = s.CreateTime, Creator = s.Creator }).Distinct().OrderByDescending(s => s.CreateTime).ToList();
            return enterpriseSet.Page(pageIndex, pageSize);
        }

        /// <summary>
        /// 根据公司ID获取企业设置信息
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public List<AttendanceItemCatagoryDTO> GetEnterpriseSetByID(string companyID)
        {
            //查询系统考勤项类别
            var List = _attendanceItemCatagoryRepository.GetEntityList().ToList();
            var systemList = _mapper.Map<List<AttendanceItemCatagoryDTO>>(List);
            //筛选出企业设置中不启用的考勤项的ID集合
            var res = _enterpriseSetRepository.GetEntityList(s => s.CompanyID == companyID);
            var itemlist = res.Where(s => s.IsEnable == false).Select(s => s.AttendanceItemID).ToList();
            //筛选出企业设置中不勾选的考勤项单位的ID集合
            var itemUnitlist = res.SelectMany(s => s.EnterpriseSetUnitList).Where(s => s.IsSelect == false).Select(s => s.AttendanceItemUnitID).ToList();
            //取出系统的考勤项
            var sysItemList = systemList.SelectMany(s => s.AttendanceItemList).ToList();
            sysItemList.ForEach(s =>
            {
                if (itemlist.Contains(s.AttendanceItemID))
                {
                    s.IsEnable = false;
                }
            });
            sysItemList.SelectMany(s=>s.AttendanceItemUnitList).ToList().ForEach(s=>{
                if (itemUnitlist.Contains(s.AttendanceItemUnitID))
                {
                    s.IsSelect = false;
                }
            });
            return systemList;
        }

        #region 企业设置的增、改
        /// <summary>
        /// 添加企业设置时，判断该公司是否存在企业设置
        /// </summary>
        /// <param name="name">班次名称</param>
        /// <returns></returns>
        public bool CheckEnterpriseSet(string companyID)
        {
            var List = _enterpriseSetRepository.GetEntityList(s => s.CompanyID == companyID, s => s.CreateTime, SortOrderEnum.Ascending).ToList();
            return List.Count > 0;
        }

        /// <summary>
        /// 添加企业设置
        /// </summary>
        /// <param name="addHolidayDTO"></param>
        /// <returns></returns>
        public bool AddEnterpriseSet(string companyID, string companyName, List<AttendanceItemCatagoryAddDTO> addEnterpriseSetList, FyuUser user)
        {
            var time = DateTime.Now;
            List<EnterpriseSet> enterpriseSetList = new List<EnterpriseSet>();
            List<EnterpriseSetUnit> enterpriseSetUnit = new List<EnterpriseSetUnit>();
            //取出添加的考勤项列表
            var ItemList = addEnterpriseSetList.SelectMany(s => s.AttendanceItemList).ToList();
            for (int i = 0; i < ItemList.Count; i++)
            {
                var EnterpriseSet = new EnterpriseSet
                {
                    EnterpriseSetID = Guid.NewGuid().ToString(),
                    CompanyID = companyID,
                    CompanyName = companyName,
                    SortNumber = i + 1,
                    CreateTime = time,
                    Creator = user.realName,
                    CreatorID = user.userId,
                    AttendanceItemID = ItemList[i].AttendanceItemID,
                    AttendanceItemName = ItemList[i].AttendanceItemName,
                    IsEnable = ItemList[i].IsEnable
                };
                //取出添加的考勤项单位列表（考勤项ID需要匹配）
                var ItemUnitList = ItemList.SelectMany(s => s.AttendanceItemUnitList).Where(s=>s.AttendanceItemID == ItemList[i].AttendanceItemID).ToList();
                for (int j = 0; j < ItemUnitList.Count; j++)
                {
                    var EnterpriseSetUnit = new EnterpriseSetUnit
                    {
                        EnterpriseSetUnitID = Guid.NewGuid().ToString(),
                        SortNumber = j + 1,
                        EnterpriseSetID = EnterpriseSet.EnterpriseSetID,
                        AttendanceItemUnitID = ItemUnitList[j].AttendanceItemUnitID,
                        AttendanceItemUnitName = ItemUnitList[j].AttendanceItemUnitName,
                        IsSelect = ItemUnitList[j].IsSelect,
                        CompanyID = companyID,
                        CompanyName = companyName
                    };
                    enterpriseSetUnit.Add(EnterpriseSetUnit);
                }
                enterpriseSetList.Add(EnterpriseSet);
            }
            _salaryUnitOfWork.BatchInsert(enterpriseSetList);
            _salaryUnitOfWork.BatchInsert(enterpriseSetUnit);
            return _salaryUnitOfWork.Commit();
        }

        /// <summary>
        /// 修改企业设置
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="EnterpriseSetDTO"></param>
        /// <returns></returns>
        public bool UpdateEnterpriseSet(string companyID, List<AttendanceItemCatagoryDTO> EnterpriseSetList)
        {
            //取出数据库的企业设置列表
            var enterpriseSetList = _enterpriseSetRepository.GetEntityList(s => s.CompanyID == companyID).ToList();
            //取出数据库的企业设置单位列表
            var enterpriseSetUnitList = enterpriseSetList.SelectMany(s => s.EnterpriseSetUnitList).ToList();
            //取出修改的考勤项列表
            var itemList = EnterpriseSetList.SelectMany(m => m.AttendanceItemList).ToList();
            //取出修改的考勤项单位列表
            var itenUnitList = itemList.SelectMany(n => n.AttendanceItemUnitList).ToList();
            enterpriseSetList.ForEach(s =>
            {
                itemList.ForEach(ss =>
                {
                    if (ss.AttendanceItemID == s.AttendanceItemID)
                    {
                        s.IsEnable = ss.IsEnable;
                    }
                });
            });
            enterpriseSetUnitList.ForEach(s =>
            {
                itenUnitList.ForEach(ss =>
                {
                    if (ss.AttendanceItemUnitID == s.AttendanceItemUnitID)
                    {
                        s.IsSelect = ss.IsSelect;
                    }
                });
            });
            _salaryUnitOfWork.BatchDelete<EnterpriseSetUnit>(s => s.CompanyID == companyID);
            _salaryUnitOfWork.BatchDelete<EnterpriseSet>(s => s.CompanyID == companyID);
            _salaryUnitOfWork.BatchInsert(enterpriseSetList);
            _salaryUnitOfWork.BatchInsert(enterpriseSetUnitList);
            return _salaryUnitOfWork.Commit();
        }
        #endregion

        public SetRuleDTO GetSetRule(FyuUser user)
        {
            var setRuleLsit = _enterpriseSetRepository.EntityQueryable<EnterpriseSet>(s => s.CompanyID == user.customerId);
            SetRuleDTO setRuleDTO = new SetRuleDTO { LataSetRule = false, AbsenceSetRule = false, EarlyLeaveSetRule = false };
            if (!setRuleLsit.Any())
            {
                return setRuleDTO;
            }
            var late = setRuleLsit.Where(s => s.AttendanceItemName == "迟到").FirstOrDefault();
            var earlyLeave = setRuleLsit.Where(s => s.AttendanceItemName == "早退").FirstOrDefault();
            var absence = setRuleLsit.Where(s => s.AttendanceItemName == "上班缺卡").FirstOrDefault();
            var absence2 = setRuleLsit.Where(s => s.AttendanceItemName == "下班缺卡").FirstOrDefault();
            if (late.IsEnable)
            {
                setRuleDTO.LataSetRule = true;
            }
            if (earlyLeave.IsEnable)
            {
                setRuleDTO.EarlyLeaveSetRule = true;
            }
            if (absence.IsEnable || absence2.IsEnable)
            {
                setRuleDTO.AbsenceSetRule = true;
            }
            return setRuleDTO;
        }
    }
}