using AutoMapper;
using Domain;
using Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class RecordBaseService
    {
        public readonly IMapper _mapper;
        public readonly IAttendanceUnitOfWork _attendanceUnitOfWork;
        public readonly ISerializer<string> _serializer;
        private readonly IEnterpriseSetRepository _enterpriseSetRepository;
        private readonly IAttendanceItemCatagoryRepository _attendanceItemCatagoryRepository;
        public RecordBaseService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IEnterpriseSetRepository enterpriseSetRepository, IAttendanceItemCatagoryRepository attendanceItemCatagoryRepository)
        {
            _attendanceUnitOfWork = attendanceUnitOfWork;
            _mapper = mapper;
            _serializer = serializer;
            _enterpriseSetRepository = enterpriseSetRepository;
            _attendanceItemCatagoryRepository = attendanceItemCatagoryRepository;
        }
        public PageResult<TDestination> PageMap<TDestination, TSource>(PageResult<TSource> a)
        {
            return new PageResult<TDestination>(a.TotalNumber, a.TotalPage, a.PageIndex, a.PageSize, _mapper.Map<List<TDestination>>(a.Data));
        }

        /// <summary>
        /// 查询企业考勤项
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<AttendanceItemForComputDTO> GetItemComputDTO(string customerId)
        {
            #region 企业设置查询
            var enterpriseSets = _enterpriseSetRepository.EntityQueryable<EnterpriseSet>(a => a.CompanyID == customerId);

            var enterpriseSetUnitList = enterpriseSets.SelectMany(s => s.EnterpriseSetUnitList).ToList();

            var attendanceItemList = _attendanceItemCatagoryRepository.EntityQueryable<AttendanceItemCatagory>(a => 1 == 1).SelectMany(s => s.AttendanceItemList).ToList();

            var query = from enterpriseSet in enterpriseSets
                        join enterpriseSetUnit in enterpriseSetUnitList on enterpriseSet.EnterpriseSetID equals enterpriseSetUnit.EnterpriseSetID
                        join attendanceitem in attendanceItemList on enterpriseSet.AttendanceItemID equals attendanceitem.AttendanceItemID
                        orderby enterpriseSet.SortNumber, enterpriseSetUnit.SortNumber
                        select new AttendanceItemForComputDTO { AttendanceItemID = enterpriseSet.AttendanceItemID, AttendanceItemName = enterpriseSet.AttendanceItemName, Unit = enterpriseSetUnit.AttendanceItemUnitName, UnitIsUsed = enterpriseSetUnit.IsSelect, AttendanceItemCatagoryID = attendanceitem.AttendanceItemCatagoryID, AttendanceItemCatagoryName = attendanceitem.AttendanceItemCatagoryName };



            #endregion
            return query.ToList();
        }
    }
}
