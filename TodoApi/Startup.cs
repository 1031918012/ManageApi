using Autofac;
using Autofac.Extensions.DependencyInjection;
using Domain;
using Infrastructure.Filter;
using ManageApi;
using ManageApi.DependenciesInit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Service;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
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
            services.AddMvc(o=> {
                o.Filters.Add(typeof(CustomExceptionFilterAttribute));
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
            services.AddScoped<RayPIToken>();
            services.AddScoped<IPeopleService, PeopleService>();
            services.AddScoped<IPeopleRepository, PeopleRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IManageService, ManageService>();
            services.AddScoped<IManageRepository, ManageRepository>();
            services.AddScoped<ISalaryUnitOfWork, SalaryUnitOfWork>();
            services.AddScoped<IRepositories<IManage>, EFRepositories<IManage>>();
            services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));
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
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("System", policy => policy.RequireClaim("System").Build());
                options.AddPolicy("Client", policy => policy.RequireClaim("Client").Build());
                options.AddPolicy("Admin", policy => policy.RequireClaim("Admin").Build());
            });
            #region autofac
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<DependenciesInit>();
            //替换完容器，构造控制器需要的参数，是autofac做的，但是控制器本身是ServiceCollection做的，包括内置的那几个
            containerBuilder.Populate(services);
            //services.AddScoped<IPeopleService, PeopleService>();
            //services.AddScoped<IPeopleRepository, PeopleRepository>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IManageService, ManageService>();
            //services.AddScoped<IManageRepository, ManageRepository>();
            //services.AddScoped<ISalaryUnitOfWork, SalaryUnitOfWork>();
            //services.AddScoped<IRepositories<IManage>, EFRepositories<IManage>>();
            //services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));
            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
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
            app.UseMiddleware<TokenAuth>();

            app.UseMvc();
        }
    }
}
