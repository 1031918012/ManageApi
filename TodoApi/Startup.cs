using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain;
using Repositories;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ManageApi.Controllers;
using System.Text;
using System.Collections.Generic;

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
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            {
                o.LoginPath = new PathString("/User/Login");
                o.AccessDeniedPath = new PathString("/Error/Forbidden");
            });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    //允许所有来源，允许所有HTTP方法，允许所有作者的请求标头
                });
            });
            services.AddDbContextPool<ManageContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("ManageConnectionStrings"),b=>b.MigrationsAssembly("Repositories"));
            });
            services.AddScoped<IPeopleService, PeopleService>();
            services.AddScoped<IPeopleRepository, PeopleRepository>();
            services.AddScoped<IManageService, ManageService>();
            services.AddScoped<IManageRepository, ManageRepository>();
            services.AddScoped<ISalaryUnitOfWork, SalaryUnitOfWork>();
            services.AddScoped<IRepositories<IManage>, EFRepositories<IManage>>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("manage", new Info { Title = "薪酬", Version = "manage" });
                c.SwaggerDoc("people", new Info { Title = "人员", Version = "people" });
                c.SwaggerDoc("user", new Info { Title = "用户", Version = "user" });
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlControllPath = Path.Combine(basePath, "ManageApi.xml");
                //var xmlDomainPath = Path.Combine(basePath, "Domain.xml");
                c.IncludeXmlComments(xmlControllPath);
                //c.IncludeXmlComments(xmlDomainPath);
                c.OperationFilter<AddAuthTokenHeaderParameter>();
            });
            
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
            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });
            app.UseSwaggerUI(c =>
            {
                c.ShowExtensions();
                c.SwaggerEndpoint("/swagger/manage/swagger.json", "薪酬");
                c.SwaggerEndpoint("/swagger/user/swagger.json", "用户");
                c.SwaggerEndpoint("/swagger/people/swagger.json", "人员");

            });
            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
