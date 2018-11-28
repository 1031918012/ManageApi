using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPost("Get")]
        public ActionResult<IEnumerable<string>> Get(int id)
        {
            return new string[] { "value1", "value2" };
        }
    }
}
