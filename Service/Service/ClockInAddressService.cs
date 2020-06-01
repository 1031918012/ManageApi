using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Service
{
    public class ClockInAddressService : BaseService, IClockInAddressService
    {
        private readonly IClockInAddressRepository _clockInAddressRepository;
        private readonly IGroupAddressRepository _groupAddressRepository;
        public ClockInAddressService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IClockInAddressRepository clockInAddressRepository, IGroupAddressRepository groupAddressRepository) : base(attendanceUnitOfWork, mapper, serializer)
        {
            _clockInAddressRepository = clockInAddressRepository;
            _groupAddressRepository = groupAddressRepository;
        }
        /// <summary>
        /// 获取公司所有打卡地点
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<ClockInAddressDTO> GetAddress(FyuUser user)
        {
            var clockInAddressList = _clockInAddressRepository.EntityQueryable<ClockInAddress>(a => a.CompanyID == user.customerId).ToList();
            var clockInAddressDTOList = _mapper.Map<List<ClockInAddressDTO>>(clockInAddressList);
            return clockInAddressDTOList;
        }
        /// <summary>
        /// 新增打卡地点
        /// </summary>
        /// <param name="clockInAddressDTO"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public JsonResponse AddClockinAddress(ClockInAddressAddDTO clockInAddressDTO, FyuUser user)
        {
            var clockInAddress = _mapper.Map<ClockInAddress>(clockInAddressDTO);

            var point = LocationUtil.BD09toGCJ02(clockInAddressDTO.LatitudeBD, clockInAddressDTO.LongitudeBD);
            clockInAddress.Latitude = point.Latitude;
            clockInAddress.Longitude = point.Longitude;

            clockInAddress.ClockInAddressID = Guid.NewGuid().ToString();
            clockInAddress.CompanyID = user.customerId;
            clockInAddress.CompanyName = user.customerName;
            clockInAddress.Creator = user.realName;
            clockInAddress.CreatorID = user.userId;
            clockInAddress.CreateTime = DateTime.Now;
            _attendanceUnitOfWork.Add(clockInAddress);
            var isSuccess = _attendanceUnitOfWork.Commit();
            return new JsonResponse { Code = CodeEnum.ok, Message = isSuccess ? "添加成功！" : "添加失败！", IsSuccess = isSuccess };
        }
        /// <summary>
        /// 修改打卡地点
        /// </summary>
        /// <param name="clockInAddressDTO"></param>
        /// <returns></returns>
        public JsonResponse UpdateClockinAddress(ClockInAddressDTO clockInAddressDTO)
        {
            var address = _clockInAddressRepository.GetEntity(s => s.ClockInAddressID == clockInAddressDTO.ClockInAddressID);

            var point = LocationUtil.BD09toGCJ02(clockInAddressDTO.LatitudeBD, clockInAddressDTO.LongitudeBD);
            address.Latitude = point.Latitude;
            address.Longitude = point.Longitude;

            address.ClockName = clockInAddressDTO.ClockName;
            address.Distance = clockInAddressDTO.Distance;
            address.LatitudeBD = clockInAddressDTO.LatitudeBD;
            address.LongitudeBD = clockInAddressDTO.LongitudeBD;
            address.SiteName = clockInAddressDTO.SiteName;

            _attendanceUnitOfWork.Update(address);
            var isSuccess = _attendanceUnitOfWork.Commit();
            return new JsonResponse { Code = CodeEnum.ok, Message = isSuccess ? "修改成功！" : "修改失败！", IsSuccess = isSuccess };
        }
        /// <summary>
        /// 删除公司地址
        /// </summary>
        /// <param name="clockinAddressID"></param>
        /// <returns></returns>
        public JsonResponse DeleteClockinAddress(string clockinAddressID)
        {
            if (string.IsNullOrEmpty(clockinAddressID))
            {
                return new JsonResponse { Code = CodeEnum.BadRequest, Message = "客户端错误", IsSuccess = false };
            }
            if (_groupAddressRepository.EntityQueryable<GroupAddress>(s => s.ClockInAddressID == clockinAddressID).Any())
            {
                return new JsonResponse { Code = CodeEnum.ok, Message = "该打卡方式正在被考勤组使用, 无法删除 !", IsSuccess = false };
            }
            _attendanceUnitOfWork.BatchDelete<ClockInAddress>(a => a.ClockInAddressID == clockinAddressID);
            var isSuccess = _attendanceUnitOfWork.Commit();
            return new JsonResponse { Code = isSuccess ? CodeEnum.ok : CodeEnum.NotFound, Message = isSuccess ? "删除成功！" : "删除失败！", IsSuccess = isSuccess };
        }

        public PageResult<ClockInAddressDTO> GetAddressPage(int pageIndex, int pageSize, FyuUser user)
        {
            Expression<Func<ClockInAddress, bool>> exp = s => s.CompanyID == user.customerId;
            var Rule = _clockInAddressRepository.GetByPage(pageIndex, pageSize, exp, s => s.CreateTime, SortOrderEnum.Descending);
            return PageMap<ClockInAddressDTO, ClockInAddress>(Rule);
        }

        public List<SupplementCardAddressDTO> GetSupplementCardAddress(string address, FyuUser user)
        {
            return _clockInAddressRepository.EntityQueryable<ClockInAddress>(s => s.CompanyID == user.customerId && (s.ClockName.Contains(address) || s.SiteName.Contains(address))).Select(s => new SupplementCardAddressDTO { SiteName = s.SiteName, ClockName = s.ClockName, ClockInAddressID = s.ClockInAddressID }).ToList();
        }
    }
}
