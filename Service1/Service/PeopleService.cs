using Domain;
using System;
using System.Collections.Generic;
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
    }
}
