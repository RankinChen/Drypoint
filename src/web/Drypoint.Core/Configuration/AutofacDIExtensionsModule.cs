using Autofac;
using Drypoint.Application.Custom.Demo;
using Drypoint.EntityFrameworkCore.EntityFrameworkCore;
using Drypoint.EntityFrameworkCore.Repositories;
using Drypoint.Unity.Dependency;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Drypoint.Core.Configuration
{
    public class AutofacDIExtensionsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<DrypointDbContextFactory>().As<IDesignTimeDbContextFactory<DrypointDbContext>>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
            builder.RegisterGeneric(typeof(DrypointBaseRepository<,>)).As(typeof(IRepository<,>)).InstancePerLifetimeScope();

            var defaultDependencyContext = Microsoft.Extensions.DependencyModel.DependencyContext.Default;
            var compileLibraries = defaultDependencyContext.CompileLibraries.Where(aa => !aa.Serviceable && aa.Type != "package" && aa.Name.StartsWith("Drypoint"));
            var listAllType = new List<Type>();
            List<Type> ltInterface = null;
            Assembly assembly = null;
            foreach (var lib in compileLibraries)
            {
                try
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                    ltInterface = assembly.GetTypes().Where(type => type != null).ToList();
                    if (ltInterface == null || ltInterface.Count < 1)
                    {
                        continue;
                    }
                    if (ltInterface.Any(aa => aa.GetInterfaces().Contains(typeof(ITransientDependency)) || aa.GetInterfaces().Contains(typeof(ISingletonDependency)) || aa.GetInterfaces().Contains(typeof(IScopedDependency))))
                    {
                        listAllType.AddRange(ltInterface);
                    }
                }
                catch { }
            }

            //注册ITransientDependency实现类
            Type iTransientDependency = typeof(ITransientDependency);
            var arrDependencyType = listAllType.Where(t => iTransientDependency.IsAssignableFrom(t) && t != iTransientDependency).ToArray();
            builder.RegisterTypes(arrDependencyType)
                .AsSelf().AsImplementedInterfaces()
                .PropertiesAutowired();

            //foreach (Type type in arrDependencyType)
            //{
            //    if (type.IsClass && !type.IsAbstract && !type.BaseType.IsInterface && type.BaseType != typeof(object))
            //    {
            //        builder.RegisterType(type).As(type.BaseType)
            //                .AsSelf().AsImplementedInterfaces()
            //                .PropertiesAutowired();
            //    }
            //}

            Type iSingletonDependency = typeof(ISingletonDependency);
            arrDependencyType = listAllType.Where(t => iSingletonDependency.IsAssignableFrom(t) && t != iSingletonDependency).ToArray();
            builder.RegisterTypes(arrDependencyType)
                   .Where(type => iSingletonDependency.IsAssignableFrom(type) && !type.IsAbstract)
                   .AsSelf().AsImplementedInterfaces()
                   .PropertiesAutowired().SingleInstance();

            Type iScopedDependency = typeof(IScopedDependency);
            arrDependencyType = listAllType.Where(t => iScopedDependency.IsAssignableFrom(t) && t != iScopedDependency).ToArray();
            builder.RegisterTypes(arrDependencyType)
                   .Where(type => iScopedDependency.IsAssignableFrom(type) && !type.IsAbstract)
                   .AsSelf().AsImplementedInterfaces()
                   .PropertiesAutowired().InstancePerLifetimeScope();
        }

    }
}
