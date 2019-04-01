using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Filter
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private ILogger<CustomExceptionFilterAttribute> _logger = null;
        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                context.HttpContext.Response.StatusCode = 500;
                _logger.LogError(context.Exception.Message);
                context.Result = new JsonResult(new {
                    Result = false,
                    PromptMsg = "系统出现异常，请联系管理员",
                    DebugMessage = context.Exception.Message
                });
            }

        }
    }
}
