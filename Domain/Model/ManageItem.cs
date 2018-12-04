using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    public class ManageItem:IManage
    {
        public Guid BookID { get; set; } 
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool Isdelete { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
    }
}
