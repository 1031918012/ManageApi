using Autofac;
using Domain;
using Microsoft.Extensions.Caching.Memory;
using Repositories;
using Service;

namespace ManageApi.DependenciesInit
{
    /// <summary>
    /// 
    /// </summary>
    public class DependenciesInit : Module
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PeopleService>().As<IPeopleService>();
            builder.RegisterType<PeopleRepository>().As<IPeopleRepository>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<ManageService>().As<IManageService>();
            builder.RegisterType<ManageRepository>().As<IManageRepository>();
            builder.RegisterType<SalaryUnitOfWork>().As<ISalaryUnitOfWork>();
            builder.RegisterType<EFRepositories<IManage>>().As<IRepositories<IManage>>();
            builder.RegisterType<MemoryCache>().As<IMemoryCache>().SingleInstance().PropertiesAutowired();
        }
    }
}
