using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IHolidayManagementService : IService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        (bool, string) Delete(Guid ID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Enable"></param>
        /// <returns></returns>
        (bool, string) EnableOrProhibit(Guid ID, bool enable);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        PageResult<HolidayManagementPageDTO> GetHolidayManagementPage(int pageIndex, int pageSize, FyuUser user);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        HolidayParamDTO GetHolidayManagement(Guid ID);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="holiday"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        (bool, string) AddHolidayManagement(HolidayParamDTO holiday, FyuUser user);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="holiday"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        (bool, string) UpdateHolidayManagement(HolidayParamDTO holiday, FyuUser user);
        (bool,string) AutoHoliday(DateTime time);
    }
}
