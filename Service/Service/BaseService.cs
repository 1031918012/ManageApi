using AutoMapper;
using Domain;
using Infrastructure;
using System.Collections.Generic;

namespace Service
{
    public class BaseService
    {
        public readonly IMapper _mapper;
        public readonly IAttendanceUnitOfWork _attendanceUnitOfWork;
        public readonly ISerializer<string> _serializer;
        public BaseService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer)
        {
            _attendanceUnitOfWork = attendanceUnitOfWork;
            _mapper = mapper;
            _serializer = serializer;
        }
        public PageResult<TDestination> PageMap<TDestination, TSource>(PageResult<TSource> a)
        {
            return new PageResult<TDestination>(a.TotalNumber, a.TotalPage, a.PageIndex, a.PageSize, _mapper.Map<List<TDestination>>(a.Data));
        }
    }
}
