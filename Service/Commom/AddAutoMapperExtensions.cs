using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Service
{
    /// <summary>
    /// 管道注册AutoMapper
    /// </summary>
    public static class AddAutoMapperExtensions
    {
        public static void AddAutoMapperAssembly(this IServiceCollection services)
        {
           var assemblies = GetAssemblies("Service").SelectMany(a => a.GetTypes().Where(t => t.BaseType == typeof(Profile))).ToArray();
           services.AddAutoMapper(assemblies);
        }
        public static List<Assembly> GetAssemblies(params string[] virtualPaths)
        {
            var referenceAssemblies = new List<Assembly>();
            if (virtualPaths.Any())
            {
                foreach (var item in virtualPaths)
                {
                    referenceAssemblies.Add(Assembly.Load(item));
                }
            }
            return referenceAssemblies;
        }
    }
    
}
