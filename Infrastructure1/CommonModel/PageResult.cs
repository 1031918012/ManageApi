using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class PageResult<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalNumber { get; set; }
        public int TotalPage { get; set; }
        public List<T> Data { get; set; }

        public PageResult(int totalNumber) { }

        public PageResult(int pageindex,int pagesize,int totalnumber, int totalpage,List<T> list)
        {
            PageIndex = pageindex;
            PageSize = pagesize;
            TotalNumber = totalnumber;
            TotalPage = totalpage;
            Data = list;
        }
    }
}
