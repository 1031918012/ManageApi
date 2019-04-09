using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class PeopleService : BaseService, IPeopleService
    {
        private readonly IPeopleRepository _people;
        public PeopleService(ISalaryUnitOfWork salaryUnitOfWork, IPeopleRepository people) : base(salaryUnitOfWork)
        {
            _people = people;
        }

        public bool AddPeople(People people)
        {
            _salaryUnitOfWork.Add(people);
            return _salaryUnitOfWork.Commit();
        }
        public bool Addpeoplelist(List<People> peoples)
        {
            _salaryUnitOfWork.AddRange(peoples);
            return _salaryUnitOfWork.Commit();
        }

        public bool Addpeoplelistasyn(List<People> peoples)
        {
            _salaryUnitOfWork.AddRangeasyn(peoples);
            return _salaryUnitOfWork.Commit();
        }

        public List<People> GetPeoples()
        {
            return _people.GetEntitieList(s => s.Name == "1").ToList();
        }
        public bool UpdateEntity(List<People> peoples)
        {
            _salaryUnitOfWork.UpdateRange(peoples);
            return _salaryUnitOfWork.Commit();
        }
        public bool UpdateEntityasync(List<People> peoples)
        {
            _salaryUnitOfWork.UpdateRangeasync(peoples);
            return _salaryUnitOfWork.Commit();
        }
    }
}
