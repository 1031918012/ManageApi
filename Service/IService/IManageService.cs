using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace Service
{
    public interface IManageService
    {
        bool Add(ManageItem manage);
        bool Update(ManageItem manage);
        ManageItem SelectEntity(Guid id);
        List<ManageItem> SelectList();
    }
}
