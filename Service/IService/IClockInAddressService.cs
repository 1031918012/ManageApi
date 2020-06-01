using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public interface IClockInAddressService : IService
    {
        List<ClockInAddressDTO> GetAddress(FyuUser user);
        JsonResponse AddClockinAddress(ClockInAddressAddDTO clockInAddressDTO, FyuUser user);
        JsonResponse DeleteClockinAddress(string clockinAddressID);
        PageResult<ClockInAddressDTO> GetAddressPage(int pageIndex, int pageSize, FyuUser user);
        JsonResponse UpdateClockinAddress(ClockInAddressDTO clockInAddressDTO);
        List<SupplementCardAddressDTO> GetSupplementCardAddress(string address, FyuUser user);
    }
}
