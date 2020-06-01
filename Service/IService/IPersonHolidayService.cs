using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IPersonHolidayService : IService
    {
        /// <summary>
        /// 查询余额人员的分页列表页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        PageResult<BalancePageDTO> GetBalancePage(int pageIndex, int pageSize, FyuUser user,string name);
        /// <summary>
        /// 同步人员
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        (bool, string) SynchroHoliday(FyuUser user);
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="holidayUpdate"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        (bool, string) UpdateList(HolidayUpdateParamDTO holidayUpdate, FyuUser user);
        /// <summary>
        /// 查询详细余额的分页接口
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <param name="IDCard"></param>
        /// <param name="holidayManagementID"></param>
        /// <returns></returns>
        PageResult<PersonHolidayPageDTO> GetPersonHolidayPage(int pageIndex, int pageSize, FyuUser user, string IDCard = "", string holidayManagementID = "");
        /// <summary>
        /// 查询流水的分页列表
        /// </summary>
        /// <param name="IDCard"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <param name="holidayManagementID"></param>
        /// <returns></returns>
        PageResult<PersonHolidayDetailPageDTO> GetPersonHolidayDetailPage(string IDCard, int pageIndex, int pageSize, FyuUser user, string holidayManagementID = "");
        /// <summary>
        /// 获取开启余额的假期列表
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        List<HolidayManagementDTO> GetHolidayList(FyuUser user);
    }
}
