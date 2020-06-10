using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace ManageApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeController : ControllerBase
    {
        private readonly IServiceProvider serviceProvider;
        /// <summary>
        /// 
        /// </summary>
        public HomeController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
    }
}
