using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public interface IHolidayService : IService
    {
        PageResult<HolidayDTO> SelectHolidayList(int pageIndex, int pageSize, int year);
        HolidayDTO GetHolidayByID(string HolidayID);
        bool CheckSameName(string name, string id, int year);
        (string,bool) AddHoliday(HolidayAddDTO addHolidayDTO);
        bool UpdateHolidayEntity(string holidayID);
        bool UpdateHoliday(HolidayDTO HolidayDTO);
    }
}
