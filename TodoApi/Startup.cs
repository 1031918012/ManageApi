using Infrastructure.Filter;
using ManageApi;
using ManageApi.DependenciesInit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TodoApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o =>
            {
                o.Filters.Add(typeof(CustomExceptionFilterAttribute));
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder                    
                    //.WithOrigins(Configuration.GetSection("Uri:FYUServer").Value, Configuration.GetSection("Uri:SalaryUI").Value)
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//允许cookie 和 header
                    //允许所有来源，允许所有HTTP方法，允许所有作者的请求标头
                });
            });
            services.AddDbContextPool<ManageContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ManageConnectionStrings"), b => b.MigrationsAssembly("Repositories"));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "ManageApi", Version = "v1" });
                c.SwaggerDoc("SalaryCommon", new Info { Title = "SalaryCommon", Version = "SalaryCommon" });
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo))
                    {
                        return false;
                    }
                    var version = methodInfo.DeclaringType.GetCustomAttributes(true).OfType<ApiExplorerSettingsAttribute>().Select(a => a.GroupName);
                    if ((docName.ToLower() == "v1") && version.FirstOrDefault() == null)
                    {
                        return true;
                    }
                    return version.Any(v => v.ToString() == docName);
                });
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlControllPath = Path.Combine(basePath, "ManageApi.xml");
                //var xmlDomainPath = Path.Combine(basePath, "Domain.xml");
                c.IncludeXmlComments(xmlControllPath);
                //c.IncludeXmlComments(xmlDomainPath);
                c.OperationFilter<AddAuthTokenHeaderParameter>();
                var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } }, };
                c.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("System", policy => policy.RequireClaim("System").Build());
                options.AddPolicy("Client", policy => policy.RequireClaim("Client").Build());
                options.AddPolicy("Admin", policy => policy.RequireClaim("Admin").Build());
            });
            #region autofac
            return AotufacInite.ConvertToAutofac(services);
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseStaticFiles();
            //app.UseSession();
            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ManageApi");
                c.SwaggerEndpoint("/swagger/SalaryCommon/swagger.json", "SalaryCommon");
            });
            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthentication();
            //app.UseMiddleware<TokenAuth>();

            app.UseMvc();
        }
    }
}
