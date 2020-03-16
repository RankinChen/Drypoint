using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Drypoint
{
    public class Program
    {
        private static string _environmentName;
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())  //注册Autofac
            //.ConfigureHostConfiguration(configHost =>
            //{
            //    configHost.SetBasePath(Directory.GetCurrentDirectory());
            //    configHost.AddJsonFile("hostsettings.json", optional: true);
            //    configHost.AddEnvironmentVariables(prefix: "DOTNET_");
            //    configHost.AddCommandLine(args);
            //})
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureLogging((hostingContext, logBuilder) =>
            {
                _environmentName = hostingContext.HostingEnvironment.EnvironmentName;
                logBuilder.AddNLog();
                NLog.LogManager.LoadConfiguration("nlog.config");
                //添加控制台日志,Docker环境下请务必启用
                logBuilder.AddConsole();
                //添加调试日志
                logBuilder.AddDebug();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                //已经默认使用Kestrel
                webBuilder
                .UseKestrel()
                .UseIISIntegration()
                .ConfigureKestrel(serverOptions =>
                {
                    serverOptions.AddServerHeader = false;
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    //var env = hostingContext.HostingEnvironment;
                    //根据环境变量加载不同的JSON配置
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{_environmentName}.json",optional: true, reloadOnChange: true);
                    //从环境变量添加配置
                    config.AddEnvironmentVariables("DOTNET_");
                }).UseStartup<Startup>();
            });

    }
}
