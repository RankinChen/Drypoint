using Autofac;
using Drypoint.EntityFrameworkCore.EntityFrameworkCore;
using Drypoint.EntityFrameworkCore.Repositories;
using Drypoint.Unity.Dependency;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Drypoint.Core.Configuration
{
    public class AutofacDIExtensionsModule : Autofac.Module
    {
        /// <summary>
        /// TODO 待研究
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        bool FilterType(string typeName, string suffix = "Service")
        {
            if (string.IsNullOrEmpty(typeName) || string.IsNullOrEmpty(suffix))
            {
                return true;
            }

            return !typeName.EndsWith(suffix);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<DrypointDbContextFactory>().As<IDesignTimeDbContextFactory<DrypointDbContext>>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
            builder.RegisterGeneric(typeof(DrypointBaseRepository<,>)).As(typeof(IRepository<,>)).InstancePerLifetimeScope();

            Assembly[] assemblies = {
                Assembly.Load("Drypoint.Core"),
                Assembly.Load("Drypoint.Unity"),
                Assembly.Load("Drypoint.Application"),
                Assembly.Load("Drypoint.Application.Custom"),
                Assembly.Load("Drypoint.EntityFrameworkCore")
            };


            Type iTransientDependency = typeof(IScopedDependency);
            builder.RegisterAssemblyTypes(assemblies)
                   .Where(type => iTransientDependency.IsAssignableFrom(type) && !type.IsAbstract && FilterType(type.Name))
                   .AsSelf().AsImplementedInterfaces()
                   .PropertiesAutowired();

            Type iSingletonDependency = typeof(ISingletonDependency);
            builder.RegisterAssemblyTypes(assemblies)
                   .Where(type => iSingletonDependency.IsAssignableFrom(type) && !type.IsAbstract && FilterType(type.Name))
                   .AsSelf().AsImplementedInterfaces()
                   .PropertiesAutowired().SingleInstance();

            Type iScopedDependency = typeof(IScopedDependency);
            builder.RegisterAssemblyTypes(assemblies)
                   .Where(type => iScopedDependency.IsAssignableFrom(type) && !type.IsAbstract && FilterType(type.Name))
                   .AsSelf().AsImplementedInterfaces()
                   .PropertiesAutowired().InstancePerLifetimeScope();

        }

    }
}
