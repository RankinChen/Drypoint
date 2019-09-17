using Drypoint.Application.Authorization;
using Drypoint.EntityFrameworkCore.EntityFrameworkCore;
using Drypoint.EntityFrameworkCore.Repositories;
using Drypoint.Host.Core.Authorization;
using Drypoint.Unity.Dependency;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Drypoint.Host.Core.Configuration
{
    /// <summary>
    /// ServiceCollection扩展类
    /// 注入实现ISingletonDependency和ITransientDependency接口的实现
    /// </summary>
    public static class ServiceCollectionDIExtensions
    {
        public static void AddServiceRegister(this IServiceCollection services)
        {
            try
            {
                services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                services.TryAddTransient(typeof(IDesignTimeDbContextFactory<DrypointDbContext>), typeof(DrypointDbContextFactory));
                services.TryAddTransient(typeof(IRepository<,>), typeof(DrypointBaseRepository<,>));
                AddCommonService(services);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void AddCommonService(IServiceCollection services)
        {
            AddService(services, "Drypoint.Host.Core");
            AddService(services, "Drypoint.Unity");
            AddService(services, "Drypoint.Application");
            AddService(services, "Drypoint.Application.Custom");
            AddService(services, "Drypoint.EntityFrameworkCore");
        }

        private static void AddService(IServiceCollection services, string assemblyName)
        {
            Assembly assembly = null;

            try
            {
                //不注入未引用的类
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception ex)
            {
                return;
            }

            List<Type> ts = assembly.GetTypes().ToList();

            foreach (var item in ts.Where(d => d.IsClass))
            {
                if (item.Namespace == null || item.IsGenericType)
                {
                    continue;
                }
                if (item.IsClass && item.Namespace.StartsWith(assemblyName))
                {

                    List<Type> ltInterface = item.GetInterfaces().Where(d => !d.IsGenericType).ToList();
                    if (ltInterface == null || ltInterface.Count < 1)
                    {
                        continue;
                    }
                    if (ltInterface.Any(aa => aa == typeof(ISingletonDependency) || aa == typeof(ITransientDependency)))
                    {
                        //如果类名和接口名之差一个I字母
                        if (ltInterface.Any(aa => aa.Name == "I" + item.Name))
                        {
                            if (ltInterface.Any(aa => aa == typeof(ISingletonDependency)))
                            {
                                Type itface = ltInterface.FirstOrDefault(aa => aa.GetType() != typeof(ISingletonDependency) && aa.Name == "I" + item.Name);
                                services.TryAddSingleton(itface, item);
                            }
                            else
                            {
                                services.TryAddTransient(ltInterface.FirstOrDefault(aa => aa.Name == "I" + item.Name), item);
                            }
                        }
                        else
                        {
                            //否则默认实现接口中第一个
                            try
                            {
                                if (ltInterface.Any(aa => aa == typeof(ISingletonDependency)))
                                {
                                    Type itface = ltInterface.FirstOrDefault(aa => aa.GetType() != typeof(ISingletonDependency));
                                    services.TryAddSingleton(itface, item);
                                }
                                else
                                {
                                    services.TryAddTransient(ltInterface.FirstOrDefault(), item);
                                }
                            }
                            finally
                            {

                            }
                        }
                    }
                    else
                    {
                        //如果实现类的名称和接口相差太大 ，手动到Startup中注入
                    }
                }

            }
        }
    }
}
