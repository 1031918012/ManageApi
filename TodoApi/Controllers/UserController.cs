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
    [Route("api/[controller]"), ApiExplorerSettings(GroupName = "user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public IManageService _service;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public UserController(IManageService service)
        {
            _service = service;
        }

    }
}