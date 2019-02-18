using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace ManageApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]"), ApiExplorerSettings(GroupName = "people")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IPeopleService _people;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="people"></param>
        public PeopleController(IPeopleService people)
        {
            _people = people;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddOnePeople")]
        public OkResult AddOnePeople(string name)
        {
            return Ok();
        } 
    }
}