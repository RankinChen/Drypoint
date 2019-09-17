using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Drypoint.Host.Core.Configuration;
using Drypoint.Unity;
using Microsoft.AspNetCore.Http;
using NLog.Extensions.Logging;
using Drypoint.Host.Core.IdentityServer;
using Newtonsoft.Json;
using IdentityServer4.AccessTokenValidation;
using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.IdentityModel.Logging;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Drypoint.Unity.Extensions;
using Drypoint.Application.Authorization;
using Drypoint.Host.Core.Authorization;
using IdentityModel;

namespace Drypoint.Host.Startup
{
    public class Startup
    {
        private readonly ILogger _logger;
        private const string LocalCorsPolicyName = "localhost";

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env, ILogger<Startup> logger)
        {
            _hostingEnvironment = env;
            _appConfiguration = env.GetAppConfiguration();
            _logger = logger;
            _logger.LogInformation($"运行环境:{env.EnvironmentName}");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //DI
            services.AddServiceRegister();

            //初始化缓存 参考 https://github.com/2881099/csredis
            CSRedisClient csredis = new CSRedisClient(_appConfiguration["RedisConnectionString"]);
            services.AddSingleton(csredis);
            services.AddSingleton<IDistributedCache>(new CSRedisCache(csredis));

            //AutoMapper 
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>();
            }, AppDomain.CurrentDomain.GetAssemblies());

            //MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(LocalCorsPolicyName));
                options.Filters.Add(typeof(AsyncAuthorizationFilter));  //添加权限过滤器
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key,按照Model中的属性名进行命名
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });

            var sbuilder = services.AddSignalR(options => { options.EnableDetailedErrors = true; });

            if (!_appConfiguration["RedisConnectionString"].IsNullOrWhiteSpace())
            {
                _logger.LogWarning("RedisConnectionString:" + _appConfiguration["RedisConnectionString"]);
                sbuilder.AddRedis(_appConfiguration["RedisConnectionString"]);
            }

            //Configure CORS for APP
            services.AddCors(options =>
            {
                options.AddPolicy(LocalCorsPolicyName, builder =>
                {
                    builder
                        //.WithOrigins(
                        //    // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                        //    _appConfiguration["App:CorsOrigins"]
                        //        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        //        .Select(o => o.RemovePostFix("/"))
                        //        .ToArray()
                        //)
                        //.SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            if (bool.Parse(_appConfiguration["App:HttpsRedirection"] ?? "false"))
            {
                //建议开启，以在浏览器显示安全图标
                //设置https重定向端口
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                    options.HttpsPort = 443;
                });
            }

            //是否启用HTTP严格传输安全协议(HSTS)
            if (bool.Parse(_appConfiguration["App:UseHsts"] ?? "false"))
            {
                services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(60);
                    options.ExcludedHosts.Add("example.com");
                });
            }

            //授权相关:资源端代码
            IdentityModelEventSource.ShowPII = true;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //资源端
            .AddIdentityServerAuthentication(options =>
            {
                //options.JwtValidationClockSkew = TimeSpan.Zero;
                options.Authority = _appConfiguration["IdentityServer:Authority"];
                options.ApiName = _appConfiguration["IdentityServer:ApiName"];
                options.ApiSecret = _appConfiguration["IdentityServer:ApiSecret"];
                options.RequireHttpsMetadata = false;
                options.JwtValidationClockSkew = TimeSpan.FromSeconds(0);  //验证token间隔时间
                //待测试
                //options.JwtBearerEvents = new JwtBearerEvents
                //{
                //    OnMessageReceived = QueryStringTokenResolver
                //};
            });

            //添加自定义API文档生成(支持文档配置)
            services.AddCustomSwaggerGen(_appConfiguration, _hostingEnvironment);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                if (bool.Parse(_appConfiguration["App:UseHsts"] ?? "false"))
                {
                    app.UseHsts();
                }
            }


            if (_appConfiguration["Logging:LogType"].ToLower() == "nlog")
            {
                loggerFactory.AddNLog();
            }

            app.UseCors(LocalCorsPolicyName); //Enable CORS!
            if (bool.Parse(_appConfiguration["App:HttpsRedirection"] ?? "false"))
            {
                _logger.LogWarning("准备启用HTTS跳转...");
                //建议开启，以在浏览器显示安全图标
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                //routes.MapHub<ChatHub>("/signalr-chat");
            });

            //授权相关:资源端代码
            app.UseAuthentication();

            //启用中间件为生成的 Swagger 规范和 Swagger UI 提供服务
            app.UseCustomSwaggerUI(_appConfiguration);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
