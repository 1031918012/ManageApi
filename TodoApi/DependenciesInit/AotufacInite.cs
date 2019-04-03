using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ManageApi.DependenciesInit
{
    /// <summary>
    /// 
    /// </summary>
    public static class AotufacInite
    {
        /// <summary>
        /// 通过反射实现自动化容器注册
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceProvider ConvertToAutofac(this IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            // containerBuilder.RegisterModule<DependenciesInit>();
            //替换完容器，构造控制器需要的参数，是autofac做的，但是控制器本身是ServiceCollection做的，包括内置的那几个
            containerBuilder.Populate(services);

            var dataAccess = Assembly.GetExecutingAssembly();
            var serviceAccess = Assembly.Load("Service");
            var infrastructureAccess = Assembly.Load("Infrastructure");
            var RepositoriesAccess = Assembly.Load("Repositories");
            var DomainAccess = Assembly.Load("Domain");

            containerBuilder.RegisterTypes(dataAccess.GetTypes());
            containerBuilder.RegisterTypes(infrastructureAccess.GetTypes());
            containerBuilder.RegisterTypes(serviceAccess.GetTypes()).AsImplementedInterfaces();
            containerBuilder.RegisterTypes(infrastructureAccess.GetTypes()).AsImplementedInterfaces();

            containerBuilder.RegisterTypes(RepositoriesAccess.GetTypes()).AsImplementedInterfaces();
            containerBuilder.RegisterTypes(DomainAccess.GetTypes()).AsImplementedInterfaces();

            var container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }
    }
}
