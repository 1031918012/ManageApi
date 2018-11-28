using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class ManageItem:IManage
    {
        public Guid ID { get; set; } 
        public string Name { get; set; }
        public string Price { get; set; }
    }
}
