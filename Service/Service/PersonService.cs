using AutoMapper;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Service
{
    public class PersonService : BaseService, IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IGroupPersonnelRepository _groupPersonnelRepository;
        private readonly IFYUServerClient _fYUServerClient;
        private readonly IEmployeeClient _employeeClient;
        private readonly ILogger _logger;
        /// <summary>
        /// 人员管理
        /// </summary>
        /// <param name="attendanceUnitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="serializer"></param>
        public PersonService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IPersonRepository personRepository, IGroupPersonnelRepository groupPersonnelRepository, IFYUServerClient fYUServerClient, IEmployeeClient employeeClient, ILogger<PersonService> logger) : base(attendanceUnitOfWork, mapper, serializer)
        {
            _personRepository = personRepository;
            _groupPersonnelRepository = groupPersonnelRepository;
            _fYUServerClient = fYUServerClient;
            _employeeClient = employeeClient;
            _logger = logger;
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="personSearchParamDTO"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public PageResult<PersonDTO> GetPersonPage(PersonSearchParamDTO personSearchParamDTO, FyuUser user)
        {
            Expression<Func<Person, bool>> exp = s => s.CompanyID == user.customerId;
            if (personSearchParamDTO.OrgType == 1)
            {
                exp = exp.And(s => s.CustomerID == personSearchParamDTO.DepartmentID);
            }
            if (personSearchParamDTO.OrgType > 1)
            {
                exp = exp.And(s => s.DepartmentID == personSearchParamDTO.DepartmentID);
            }
            if (!string.IsNullOrEmpty(personSearchParamDTO.PositionID))
            {
                exp = exp.And(s => s.PositionID == personSearchParamDTO.PositionID);
            }
            if (!string.IsNullOrEmpty(personSearchParamDTO.IDCard))
            {
                exp = exp.And(s => s.IDCard.Contains(personSearchParamDTO.IDCard));
            }
            if (!string.IsNullOrEmpty(personSearchParamDTO.IDType))
            {
                exp = exp.And(s => s.IDType.Contains(personSearchParamDTO.IDType));
            }
            if (!string.IsNullOrEmpty(personSearchParamDTO.Name))
            {
                exp = exp.And(s => s.Name.Contains(personSearchParamDTO.Name));
            }
            if (personSearchParamDTO.State != 0)
            {
                exp = exp.And(s => s.IsBindWechat == (WechatBindEnum)personSearchParamDTO.State);
            }
            var person = _personRepository.EntityQueryable(exp);
            var groupPersonnel = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(s => s.CompanyID == user.customerId);
            var query = from s in person
                        join c in groupPersonnel on s.IDCard equals c.IDCard into gc
                        from gci in gc.DefaultIfEmpty()
                        select new PersonDTO
                        {
                            PersonID = s.PersonID,
                            IDCard = s.IDCard,
                            CompanyID = s.CompanyID,
                            CompanyName = s.CompanyName,
                            Department = s.Department,
                            GroupID = gci.AttendanceGroupID,
                            IDType = s.IDType,
                            IsBindWechat = (int)s.IsBindWechat,
                            JobNumber = s.JobNumber,
                            Name = s.Name,
                            PhoneCode = s.PhoneCode,
                            Position = s.Position,
                            Hiredate = s.Hiredate,
                            StartJobTime = s.StartJobTime,
                            CustomerID = s.CustomerID,
                            DepartmentID = s.DepartmentID,
                            PositionID = s.PositionID,
                            Sex = s.Sex,
                            IsBindWechatConvert = s.IsBindWechat.GetDescription(),
                        };
            var a = query.ToList();
            return a.Page(personSearchParamDTO.PageIndex, personSearchParamDTO.PageSize);
        }

        /// <summary>
        /// 新增判断
        /// </summary>
        /// <param name="personDTO"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddPerson(PersonAddDTO personDTO, FyuUser user)
        {
            Person person = new Person
            {
                CompanyID = user.customerId,
                CompanyName = personDTO.CompanyName,
                CustomerID = personDTO.CustomerID,
                Department = personDTO.Department,
                DepartmentID = personDTO.DepartmentID,
                Hiredate = personDTO.Hiredate,
                IDCard = personDTO.IDCard.ToLower(),
                IDType = personDTO.IDType,
                IsBindWechat = string.IsNullOrEmpty(_fYUServerClient.GetOpenID(personDTO.IDCard.ToUpper())) ? WechatBindEnum.Unbound : WechatBindEnum.Bind,
                IsSynchroHoliday = false,
                JobNumber = personDTO.JobNumber,
                Name = personDTO.Name,
                PersonID = Guid.NewGuid().ToString(),
                PhoneCode = personDTO.PhoneCode,
                Position = personDTO.Position,
                PositionID = personDTO.PositionID,
                Sex = (int.Parse(personDTO.IDCard.Substring(14, 3)) % 2 == 0 ? 2 : 1),
                StartJobTime = (DateTime)personDTO.StartJobTime,
            };
            _attendanceUnitOfWork.Add(person);
            var persons = new List<Person> { person };
            var employee = BuildEmployee(persons, user);
            var json = _fYUServerClient.AddEmployee(employee);
            _logger.LogError(json.Item2);
            if (!json.Item1)
            {
                return false;
            }
            return _attendanceUnitOfWork.Commit();
        }

        /// <summary>
        /// 删除人员
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        public bool DeletePerson(string personID)
        {
            var idcard = _personRepository.EntityQueryable<Person>(a => a.PersonID == personID).Select(a => a.IDCard).FirstOrDefault();
            _attendanceUnitOfWork.BatchDelete<Person>(a => a.PersonID == personID);
            _attendanceUnitOfWork.BatchDelete<GroupPersonnel>(a => a.IDCard == idcard);
            return _attendanceUnitOfWork.Commit();
        }

        /// <summary>
        /// 更新人员
        /// </summary>
        /// <param name="personDTO"></param>
        /// <param name="fyuUser"></param>
        /// <returns></returns>
        public bool UpdatePerson(PersonDTO personDTO, FyuUser fyuUser)
        {
            var person = _personRepository.EntityQueryable<Person>(a => a.PersonID == personDTO.PersonID).FirstOrDefault();
            person.CompanyID = fyuUser.customerId;
            person.CustomerID = personDTO.CustomerID;
            person.CompanyName = personDTO.CompanyName;
            person.Department = personDTO.Department;
            person.DepartmentID = personDTO.DepartmentID;
            person.IDType = personDTO.IDType;
            person.JobNumber = personDTO.JobNumber;
            person.Name = personDTO.Name;
            person.PhoneCode = personDTO.PhoneCode;
            person.Position = personDTO.Position;
            person.PositionID = personDTO.PositionID;
            person.Hiredate = personDTO.Hiredate;
            person.StartJobTime = personDTO.StartJobTime ?? personDTO.Hiredate;
            if (!IsGroupPersonnelExist(personDTO.IDCard))
            {
                person.IDCard = personDTO.IDCard.ToLower();
            }
            _attendanceUnitOfWork.Update(person);
            return _attendanceUnitOfWork.Commit();
        }

        /// <summary>
        /// 人员信息表模版导出
        /// </summary>
        /// <returns></returns>
        public byte[] GetPersonInfoTemplate()
        {
            var color = new AllColor(new RGBColor(11, 255, 153, 153), null, new RGBColor(12, 0, 0, 0));
            //表格内容
            var title = new List<Diction>()
            {
                new Diction( "JobNumber", "工号(必填)",color),
                new Diction( "Name", "姓名(必填)",color),
                new Diction( "CompanyName", "所属公司名称(必填)",color),
                new Diction( "Department", "部门(必填)",color),
                new Diction( "Position", "职位(必填)",color),
                new Diction( "IDType", "证照类型(必填)",color),
                new Diction(  "IDCard", "证照号码(必填)" ,color),
                new Diction( "PhoneCode", "手机号码(必填)",color),
                new Diction( "Hiredate", "入职日期(必填)",color),
                new Diction( "StartJobTime", "首次参加工作时间",color)
            };
            return NPOIHelp.OutputExcelXSSF(new NPIOExtend<Person>(null, title, "人员信息模板"));
        }

        private (List<Person>, List<Person>) GetOrganizationMessage(List<Person> modifiedPersons, string customerID)
        {
            ExecuteResult organization = _fYUServerClient.GetDepartMent(customerID, true);
            ExecuteResult department = _fYUServerClient.GetDepartMent(customerID, false);
            Dictionary<string, ExecuteResult> keyValuePairs = new Dictionary<string, ExecuteResult>();
            keyValuePairs = GetDepartmentMessage(new List<ExecuteResult> { organization }, keyValuePairs);
            Dictionary<string, ExecuteResult> keyValuePairs2 = new Dictionary<string, ExecuteResult>();
            keyValuePairs2 = GetDepartmentMessage(new List<ExecuteResult> { department }, keyValuePairs2);
            List<JobPositionDTO> jobList = _employeeClient.JobPosition(customerID).Data;
            List<Person> failList = new List<Person>();
            List<Person> successList = new List<Person>();
            foreach (Person item in modifiedPersons)
            {
                if (!keyValuePairs.TryGetValue(item.CompanyName,out ExecuteResult execute))
                {
                    failList.Add(item);
                    continue;
                }
                item.CustomerID = execute.id;
                if (!keyValuePairs2.TryGetValue(item.CompanyName,out  ExecuteResult execute2))
                {
                    failList.Add(item);
                    continue;
                }
                item.DepartmentID = execute2.id;
                item.PositionID = jobList.Where(S => S.jobPositionName == item.Position).FirstOrDefault()?.jobPositionID;
                if (item.PositionID == null)
                {
                    failList.Add(item);
                    continue;
                }
                successList.Add(item);
            }
            return (successList, failList);
        }
        private Dictionary<string, ExecuteResult> GetDepartmentMessage(List<ExecuteResult> organization, Dictionary<string, ExecuteResult> keyValuePairs)
        {
            foreach (var item in organization)
            {
                if (item.children.Any())
                {
                    keyValuePairs = GetDepartmentMessage(item.children, keyValuePairs);
                }
                keyValuePairs.TryAdd(item.label, item);
            }
            return keyValuePairs;
        }
        /// <summary>
        /// 导入员工基本信息、银行卡信息(人员信息中)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public (bool, string, int) ImportPersonInfo(List<PersonImportDTO> personDTOs, FyuUser user)
        {
            var persons = _mapper.Map<List<Person>>(personDTOs);
            var idCards = persons.Select(a => a.IDCard).ToList();
            var modifiedPersons = _personRepository.EntityQueryable<Person>(a => idCards.Contains(a.IDCard) && a.CompanyID == user.customerId).ToList();
            var modifiedIdCards = modifiedPersons.Select(a => a.IDCard);
            if (modifiedPersons.Any())
            {
                modifiedPersons = GetOrganizationMessage(modifiedPersons, user.customerId).Item1;
                foreach (var modifiedPerson in modifiedPersons)
                {
                    var person = persons.Where(a => a.IDCard == modifiedPerson.IDCard).FirstOrDefault();
                    if (person != null)
                    {
                        modifiedPerson.CompanyID = user.customerId;
                        modifiedPerson.CompanyName = person.CompanyName;
                        modifiedPerson.Department = person.Department;
                        modifiedPerson.IDType = person.IDType;
                        modifiedPerson.JobNumber = person.JobNumber;
                        modifiedPerson.Name = person.Name;
                        modifiedPerson.PhoneCode = person.PhoneCode;
                        modifiedPerson.Position = person.Position;
                        modifiedPerson.Hiredate = person.Hiredate;
                        modifiedPerson.StartJobTime = person.StartJobTime;
                    }
                }
                _attendanceUnitOfWork.Update(modifiedPersons);
            }
            int i = 0;
            StringBuilder message = new StringBuilder();
            List<Person> res = new List<Person>();
            List<Person> newPersons = persons.Where(a => !modifiedIdCards.Contains(a.IDCard)).ToList();
            if (newPersons.Any())
            {
                var result = GetOrganizationMessage(newPersons, user.customerId);
                newPersons = result.Item1;
                i = result.Item2.Count;
                foreach (var newPerson in newPersons)
                {
                    if (IsPersonExist(newPerson.IDCard, null))
                    {
                        message = message.Append(newPerson.IDCard + "，");
                        i++;
                        continue;
                    }
                    newPerson.PersonID = Guid.NewGuid().ToString();
                    newPerson.CompanyID = user.customerId;
                    newPerson.IsSynchroHoliday = false;
                    newPerson.IDCard = newPerson.IDCard.ToLower();
                    newPerson.Sex = (int.Parse(newPerson.IDCard.Substring(14, 3)) % 2 == 0 ? 2 : 1);
                    newPerson.IsBindWechat = string.IsNullOrEmpty(_fYUServerClient.GetOpenID(newPerson.IDCard.ToUpper())) ? WechatBindEnum.Unbound : WechatBindEnum.Bind;//检查
                    res.Add(newPerson);
                }
                _attendanceUnitOfWork.BatchInsert(res);
            }
            if (res.Any())
            {
                var employee = BuildEmployee(res, user);
                var json = _fYUServerClient.AddEmployee(employee);
                _logger.LogError(json.Item2);
                if (!json.Item1)
                {
                    return (false, "由于网络原因导入失败，请重新导入", i);
                }
            }
            var result2 = _attendanceUnitOfWork.Commit();
            return (result2, string.IsNullOrEmpty(message.ToString().TrimEnd('，')) ? "" : message.ToString().TrimEnd('，') + "已在本公司或其他公司存在，无法导入。", i);
        }


        #region 非人员展示页面相关
        /// <summary>
        /// 根据身份证号查询员工信息
        /// </summary>
        /// <param name="IDCardList"></param>
        /// <returns></returns>
        public List<PersonDTO> GetPersonByIDCard(List<string> IDCardList, FyuUser user)
        {
            var list = _personRepository.EntityQueryable<Person>(s => IDCardList.Contains(s.IDCard) && s.CompanyID == user.customerId);
            return _mapper.Map<List<PersonDTO>>(list);
        }
        public PersonStatisticsDTO GetPersonStatistics(FyuUser fyuUser)
        {
            var person = _personRepository.EntityQueryable<Person>(t => t.CompanyID == fyuUser.customerId);
            var groupPersonnel = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(s => s.CompanyID == fyuUser.customerId);
            var query = from s in person
                        join c in groupPersonnel on s.IDCard equals c.IDCard into gc
                        from gci in gc.DefaultIfEmpty()
                        select new PersonDTO
                        {
                            GroupID = gci.AttendanceGroupID,
                            IsBindWechat = (int)s.IsBindWechat
                        };
            var a = query.ToList();
            return new PersonStatisticsDTO
            {
                PeopleCount = a.Count(),
                BindWeChatCount = a.Where(t => t.IsBindWechat == 1).Count(),
                NotBindWeChatCount = a.Where(t => t.IsBindWechat == 2).Count(),
                AddedAttendanceGroupCount = a.Where(t => !string.IsNullOrEmpty(t.GroupID)).Count(),
                NotAddedAttendanceGroupCount = a.Where(t => string.IsNullOrEmpty(t.GroupID)).Count()
            };
        }
        private List<EM_EMPLOYEE> BuildEmployee(List<Person> people, FyuUser user)
        {
            List<EM_EMPLOYEE> employee = new List<EM_EMPLOYEE>();
            foreach (var item in people)
            {
                var employeeone = new EM_EMPLOYEE
                {
                    birthDay = DateTime.Parse(item.IDCard.Substring(6, 4) + "-" + item.IDCard.Substring(10, 2) + "-" + item.IDCard.Substring(12, 2)),
                    city = string.Empty,
                    credentialsType = 1,
                    customerId = user.customerId,
                    email = string.Empty,
                    gender = item.Sex,
                    idNumber = item.IDCard,
                    isJoinSS = 0,
                    maritalStatus = 0,
                    mobilePhone = item.PhoneCode,
                    nation = string.Empty,
                    politicsStatus = string.Empty,
                    realName = item.Name,
                    serviceTime = DateTime.Now
                };
                employee.Add(employeeone);
            }
            return employee;
        }
        public List<CompanyForTreeDTO> GetOrganization(FyuUser fyuUser)
        {
            List<CompanyForTreeDTO> companyForTreeDTOs = new List<CompanyForTreeDTO>();
            CompanyForTreeDTO companyForTreeDTO = new CompanyForTreeDTO
            {
                Id = fyuUser.customerId,
                Name = fyuUser.customerName
            };
            var persons = _personRepository.EntityQueryable<Person>(t => t.CompanyID == fyuUser.customerId).ToList();
            var departments = persons.Select(t => t.Department).Distinct().ToList();
            List<DepartmentForTreeDTO> departmentForTreeDTOs = new List<DepartmentForTreeDTO>();
            foreach (var department in departments)
            {
                var person = persons.Where(a => a.Department == department).ToList();
                var personDTO = _mapper.Map<List<PersonDTO>>(person);
                DepartmentForTreeDTO departmentForTreeDTO = new DepartmentForTreeDTO
                {
                    Id = department,
                    Name = department,
                    Children = personDTO
                };
                departmentForTreeDTOs.Add(departmentForTreeDTO);
            }
            companyForTreeDTO.Children = departmentForTreeDTOs;
            companyForTreeDTOs.Add(companyForTreeDTO);
            return companyForTreeDTOs;
        }
        public bool GetPersonWeChatBind()
        {
            List<string> idcard = _personRepository.GetEntityList(s => s.IsBindWechat == WechatBindEnum.Unbound).Select(s => s.IDCard.ToUpper()).ToList();
            if (!idcard.Any())
            {
                return false;
            }
            var res = _fYUServerClient.GetOpenIdRange(idcard);
            if (res == null)
            {
                return false;
            }
            for (int i = 0; i < idcard.Count; i++)
            {
                string value = string.Empty;
                res.TryGetValue(idcard[i], out value);
                if (!string.IsNullOrEmpty(value))
                {
                    _attendanceUnitOfWork.BatchUpdate<Person>(s => new Person() { IsBindWechat = WechatBindEnum.Bind }, s => s.IDCard == idcard[i].ToLower());
                }
            }
            return _attendanceUnitOfWork.Commit();
        }
        public bool IsPersonExistByPersonID(string personID)
        {
            return _personRepository.EntityQueryable<Person>(a => a.PersonID == personID, true).Any();
        }
        /// <summary>
        /// 新增人员时校验是否存在
        /// </summary>
        /// <param name="IdCard"></param>
        /// <returns></returns>
        public bool IsPersonExist(string IdCard, FyuUser user, int mycompany = 0)
        {
            Expression<Func<Person, bool>> exp = a => a.IDCard == IdCard;
            if (mycompany == 1)
            {
                exp = exp.And(a => a.CompanyID == user.customerId);
            }
            if (mycompany == 2)
            {
                exp = exp.And(a => a.CompanyID != user.customerId);
            }
            return _personRepository.EntityQueryable(exp, true).Any();
        }
        /// <summary>
        /// 更新校验身份证重复
        /// </summary>
        /// <param name="IdCard"></param>
        /// <param name="personID"></param>
        /// <returns></returns>
        public bool IsPersonExist(string IdCard, string personID)
        {
            return _personRepository.EntityQueryable<Person>(a => a.IDCard == IdCard && a.PersonID != personID, true).Any();
        }
        /// <summary>
        /// 判断人员是否存在考情组里
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public bool IsGroupPersonnelExist(string idCard)
        {
            return _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(a => a.IDCard == idCard).Any();
        }

        //public List<ExecuteResultDTO> GetOrganizationNew(FyuUser user)
        //{
        //    ExecuteResult a = _fYUServerClient.GetDepartMent(user.customerId, false);
        //    List<PersonSelectDTO> person = _personRepository.EntityQueryable<Person>(s => s.CompanyID == user.customerId, true).Select(s => new PersonSelectDTO { IDCard = s.IDCard, Name = s.Name, DepartmentID = s.DepartmentID }).ToList();
        //    ExecuteResultDTO executeResultDTO = _mapper.Map<ExecuteResultDTO>(a);
        //    List<ExecuteResultDTO> ExecuteResult = new List<ExecuteResultDTO> { executeResultDTO };
        //    return GetOrganizationPerson(ExecuteResult, person);
        //}

        //private List<ExecuteResultDTO> GetOrganizationPerson(List<ExecuteResultDTO> executeResult, List<PersonSelectDTO> person)
        //{
        //    foreach (ExecuteResultDTO item in executeResult)
        //    {
        //        item.Person = person.Where(s => s.DepartmentID == item.Id).ToList();
        //        if (item.Children.Count >= 0)
        //        {
        //            GetOrganizationPerson(item.Children, person);
        //        }
        //    }
        //    return executeResult;
        //}

        public List<ExecuteResult2DTO> GetOrganizationNew(FyuUser user)
        {
            ExecuteResult a = _fYUServerClient.GetDepartMent(user.customerId, false);
            List<PersonSelectDTO> person = _personRepository.EntityQueryable<Person>(s => s.CompanyID == user.customerId, true).Select(s => new PersonSelectDTO { IDCard = s.IDCard, Name = s.Name, DepartmentID = s.DepartmentID }).ToList();
            List<ExecuteResult2DTO> personDto = _mapper.Map<List<ExecuteResult2DTO>>(person);
            ExecuteResult2DTO executeResultDto = _mapper.Map<ExecuteResult2DTO>(a);
            List<ExecuteResult2DTO> ExecuteResult = new List<ExecuteResult2DTO> { executeResultDto };
            return GetOrganizationPerson2(ExecuteResult, personDto);
        }

        private List<ExecuteResult2DTO> GetOrganizationPerson2(List<ExecuteResult2DTO> executeResult, List<ExecuteResult2DTO> person)
        {
            foreach (ExecuteResult2DTO item in executeResult)
            {
                List<ExecuteResult2DTO> idlist = person.Where(s => s.Id == item.Id).ToList();
                if (item.Children.Where(s => s.IsDepartment == true).Count() > 0)
                {
                    GetOrganizationPerson2(item.Children, person);
                }
                item.Children.AddRange(idlist);
            }
            return executeResult;
        }

        public List<SupplementCardDTO> GetSupplementCard(string nameOrIDCard, FyuUser user)
        {
            return _personRepository.EntityQueryable<Person>(s => s.CompanyID == user.customerId && s.Name.Contains(nameOrIDCard) || s.IDCard.Contains(nameOrIDCard)).Select(s => new SupplementCardDTO { Name = s.Name, Id = s.PersonID, IDCard = s.IDCard }).ToList();
        }

        #endregion
    }
}
