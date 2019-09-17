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
using Microsoft.Extensions.Caching.Distributed;
using IdentityServer4;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IdentityServer4.Validation;
using IdentityServer4.Services;

namespace Drypoint.SSO
{
    public partial class Startup
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

        /// <summary>
        /// 配置自定义服务
        /// </summary>
        /// <param name="services"></param>
        partial void ConfigureCustomServices(IServiceCollection services);

        partial void CustomConfigure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory);

        public void ConfigureServices(IServiceCollection services)
        {
            //DI
            services.AddServiceRegister();

            //MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(LocalCorsPolicyName));
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

            //Configure CORS for APP
            services.AddCors(options =>
            {
                options.AddPolicy(LocalCorsPolicyName, builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            //设置https重定向端口
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443;
            });


            //授权相关：服务端代码
            services.AddIdentityServer(options =>
            {
                ////用户交互配置 主要涉及到入口地址参数等
                //options.UserInteraction = new IdentityServer4.Configuration.UserInteractionOptions
                //{
                //    LoginUrl = "/Account/Login",
                //    LogoutUrl = "/Account/Logout",
                //    ConsentUrl = "/Account/Consent",
                //    //ErrorUrl = "/Account/Error",
                //    LoginReturnUrlParameter = "ReturnUrl",
                //    LogoutIdParameter = "logoutId",
                //    ConsentReturnUrlParameter = "ReturnUrl",
                //    ErrorIdParameter = "errorId",
                //    CustomRedirectReturnUrlParameter = "ReturnUrl",
                //    CookieMessageThreshold = 5
                //};
            }).AddDeveloperSigningCredential()        //使用演示签名证书
            //.AddSigningCredential(new X509Certificate2(Path.Combine(AppContext.BaseDirectory, Configuration["Certs:Path"]), Configuration["Certs:Pwd"]))
              .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources(_appConfiguration))
              .AddInMemoryApiResources(IdentityServerConfig.GetApiResources(_appConfiguration))
              .AddInMemoryClients(IdentityServerConfig.GetClients(_appConfiguration))
              //使用内存测试数据身份认证 TODO
              .AddTestUsers(IdentityServerConfig.GetTestUser())
              //添加自定义claim
              .AddProfileService<ProfileService>()
              //自定义用户密码验证模式
              .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

            /*
            services.AddAuthentication()
                //开启google登陆
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    //register your IdentityServer with Google at https://console.developers.google.com
                    //enable the Google + API
                    //set the redirect URI to http://localhost:5000/signin-google
                    options.ClientId = "xxxxxxxxxxxxxxxxxm";
                    options.ClientSecret = "xxxxxxxxxxxxxxxxx";
                });
            */
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
                app.UseHsts();
            }


            if (_appConfiguration["Logging:LogType"].ToLower() == "nlog")
            {
                loggerFactory.AddNLog();
            }

            app.UseCors(LocalCorsPolicyName); //Enable CORS!

            //开启HTTPS重定向
            app.UseHttpsRedirection();


            //授权相关：服务端代码
            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}
