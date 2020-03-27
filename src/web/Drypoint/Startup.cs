using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Drypoint.Core.Configuration;
using Microsoft.AspNetCore.Http;
using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using AutoMapper;
using Drypoint.Core.Authorization;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Autofac;
using Drypoint.Core.Authentication;
using System.Linq;
using Drypoint.Application.AutoMapper;

namespace Drypoint
{
    public class Startup
    {
        readonly string LocalCorsPolicyName = "localhost";
        public IConfiguration Configuration { get; }
        private IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Environment = environment;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //配置选项模式读取配置文件
            services.AddCustomOptions(Configuration);

            services.AddHttpClient();
            //AutoMapper 
            #region AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>();
            }, AppDomain.CurrentDomain.GetAssemblies());
            #endregion

            #region CSRedisCache
            //初始化缓存 参考 https://github.com/2881099/csredis
            CSRedisClient csredis = new CSRedisClient(Configuration["RedisConnectionString"]);
            services.AddSingleton(csredis);
            services.AddSingleton<IDistributedCache>(new CSRedisCache(csredis));
            #endregion

            #region MVC
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                //options.Filters.Add(new CorsAuthorizationFilterFactory(LocalCorsPolicyName));
                options.Filters.Add(typeof(AsyncAuthorizationFilter));  //添加权限过滤器
            }).AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key,按照Model中的属性名进行命名
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            #endregion

            #region CORS
            services.AddCors(options =>
            {
                options.AddPolicy(LocalCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(Configuration["App:CorsOrigins"]
                                    .Split(",", StringSplitOptions.RemoveEmptyEntries).ToArray()
                        )
                        //.AllowAnyOrigin();
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        //.SetIsOriginAllowed(ori => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); //不要和AllowAnyOrigin同时使用
                });
            });
            #endregion

            //设置https重定向端口
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443;
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                options.ExcludedHosts.Add("example.com");
            });

            //扩展方法 注册IdentityServer或者JWT认证
            AuthConfigurer.Configure(services, Configuration);

            //添加自定义API文档生成(支持文档配置)
            services.AddCustomSwaggerGen(Configuration);
        }

        /// <summary>
        /// Autofac执行注入的地方 ConfigureServices之后执行
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacDIExtensionsModule());
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "text/html";
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        if (ex != null)
                        {
                            var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace}";
                            context.Response.Headers.Add("application-error", ex.Error.Message);
                            context.Response.Headers.Add("access-control-expose-headers", "application-error");
                            context.Response.Headers.Add("access-control-allow-origin", "*");
                            await context.Response.WriteAsync(err).ConfigureAwait(false);
                        }
                    });
                });
                app.UseHsts();
            }

            //CORS
            app.UseCors(LocalCorsPolicyName);

            //开启HTTPS重定向
            app.UseHttpsRedirection();

            //访问静态文件
            app.UseStaticFiles();
            //压缩 由于UseStaticFiles在之前 故不压缩静态文件
            //app.UseResponseCompression();
            //app.UseCookiePolicy();
            //app.UseRouting();

            //授权相关:资源端代码
            app.UseAuthentication();

            //app.UseAuthorization();
            //app.UseSession();
            //启用中间件为生成的 Swagger 规范和 Swagger UI 提供服务
            app.UseCustomSwaggerUI(Configuration);

            app.UseMvc();
        }
    }
}
