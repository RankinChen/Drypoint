using Drypoint.Unity;
using Drypoint.Unity.OptionsConfigModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Core.Configuration
{
    public static class OptionsConfig
    {
        /// <summary>
        /// 使用选项模式绑定分层配置数据
        /// 参考 https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#default-configuration
        /// 关于IOptions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.Configure<AuthManagement>(configuration.GetSection("Authentication"));


        }
    }
}
