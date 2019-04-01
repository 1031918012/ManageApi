using Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ManageApi
{
    /// <summary>
    /// Token验证授权中间件
    /// </summary>
    public class TokenAuth
    {
        /// <summary>
        /// http委托
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next"></param>
        public TokenAuth(RequestDelegate next)
        {
            _next = next;
        }
        /// <summary>
        /// 验证授权
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.HasValue)
            {
                if (httpContext.Request.Path.Value.Contains("User"))
                {
                    return _next(httpContext);
                }
            }
            //前端请求会发送两次，请求为OPTIONS时不做处理
            if (httpContext.Request.Method != "OPTIONS")
            {
                var headers = httpContext.Request.Headers;
                //检测是否包含'Authorization'请求头，如果不包含返回context进行下一个中间件，用于访问不需要认证的API
                if (!headers.ContainsKey("Authorization"))
                {
                    return _next(httpContext);
                }
                var tokenStr = headers["Authorization"];
                try
                {
                    //验证缓存中是否存在该jwt字符串
                    if (!RayPIMemoryCache.Exists(tokenStr))
                    {
                        httpContext.Response.StatusCode = 301;
                        httpContext.Response.WriteAsync("登陆缓存验证字符串已经过期");
                        return Task.CompletedTask;
                    }
                    User tm = (User)RayPIMemoryCache.Get(tokenStr);
                    //提取tokenModel中的Sub属性进行authorize认证
                    List<Claim> lc = new List<Claim>();
                    Claim c = new Claim(tm.Sub, tm.Sub);
                    lc.Add(c);
                    ClaimsIdentity identity = new ClaimsIdentity(lc);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    httpContext.User = principal;
                    return _next(httpContext);
                }
                catch (Exception)
                {
                    return httpContext.Response.WriteAsync("token验证异常");
                }
            }
            return _next(httpContext);

        }
    }
}