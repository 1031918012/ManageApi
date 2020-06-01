using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories
{
    public class DayTableCalculationRepository : EFRepository<DayTableCalculation>, IDayTableCalculationRepository, IBaseRepository
    {
        public DayTableCalculationRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
