using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace TodoApi
{
    /// <summary>
    /// 
    /// </summary>
    public class AddAuthTokenHeaderParameter : AuthorizeAttribute,IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<IParameter>();
            }
            var attrs = context.ApiDescription.ActionDescriptor.AttributeRouteInfo;

            //先判断是否是匿名访问,
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                object[] actionAttributes = descriptor.MethodInfo.GetCustomAttributes(inherit: true);
                bool isAnonymous = actionAttributes.Any(a => a is AllowAnonymousAttribute);
                //非匿名的方法,链接中添加accesstoken值
                if (!isAnonymous)
                {
                    operation.Parameters.Add(new NonBodyParameter()
                    {
                        Name = "Claim",
                        In = "query",//query header body path formData
                        Type = "apiKey",
                        Description = "请求客户端来源",
                    });
                }
            }
        }
    }
}