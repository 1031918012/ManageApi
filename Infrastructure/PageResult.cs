using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalNumber"></param>
        /// <param name="totalPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="data"></param>
        public PageResult(int totalNumber, int totalPage, int pageIndex, int pageSize, List<T> data)
        {
            this.TotalNumber = totalNumber;
            this.TotalPage = totalPage;
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.Data = data;
        }
        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalNumber { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> Data { get; set; }

    }
    /// <summary>
    /// 分页查询拓展
    /// </summary>
    public static class PageExtension
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PageResult<T> Page<T>(this List<T> a, int pageIndex, int pageSize)
        {
            int totalNumber = a.Count();
            List<T> list = a.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PageResult<T>(totalNumber, (totalNumber + pageSize - 1) / pageSize, pageIndex , pageSize, list);
        }

    }
}
