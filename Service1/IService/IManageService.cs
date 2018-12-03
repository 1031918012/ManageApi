using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace Service
{
    public interface IManageService
    {
        bool Add(ManageItem manage);
        List<ManageItem> SelectList();
    }
}
