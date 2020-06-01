using AutoMapper;
using Domain;
using Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Service
{
    public class GroupPersonnelService : IGroupPersonnelService
    {
        private readonly IGroupPersonnelRepository _groupPersonnelRepository;
        private readonly IAttendanceUnitOfWork _salaryUnitOfWork;
        public GroupPersonnelService(IAttendanceUnitOfWork salaryUnitOfWork, IGroupPersonnelRepository groupPersonnelRepository)
        {
            _salaryUnitOfWork = salaryUnitOfWork;
            _groupPersonnelRepository = groupPersonnelRepository;
        }

        public FYWDataResult<string> GetAllActiveUsers(string customerId)
        {
            var allUser = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(s => s.CompanyID == customerId).Select(s=>s.IDCard).ToList();
            return new FYWDataResult<string> { code = 200, count = allUser.Count, data = allUser, msg = "Success" };
        }
    }
}
