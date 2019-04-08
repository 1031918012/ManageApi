using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public interface IPeopleService
    {
        bool AddPeople(People people);
        bool Addpeoplelist(List<People> peoples);

        bool Addpeoplelistasyn(List<People> peoples);

    }
}
