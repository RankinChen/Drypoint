using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Drypoint.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Drypoint.Core.IdentityServer;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using AutoMapper;
using System;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Autofac;
using Drypoint.Application.AutoMapper;

namespace Drypoint.SSO
{
    public partial class Startup
    {
        private const string LocalCorsPolicyName = "localhost";

        public IConfiguration Configuration { get; }
        //IWebHostEnvironment继承了IHostEnvironment 添加两个关于Web根目录的属性
        private IHostEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            //AutoMapper 
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>();
            }, AppDomain.CurrentDomain.GetAssemblies());

            //DI
            //services.AddServiceRegister();

            //MVC
            services.AddControllersWithViews();
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                //options.Filters.Add(new CorsAuthorizationFilterFactory(LocalCorsPolicyName));
            }).AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key,按照Model中的属性名进行命名
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";

            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //Configure CORS for APP
            services.AddCors(options =>
            {
                options.AddPolicy(LocalCorsPolicyName, builder =>
                {
                    builder
                        //.WithOrigins(
                        //    // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                        //    Configuration["App:CorsOrigins"]
                        //        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        //        .Select(o => o.RemovePostFix("/"))
                        //        .ToArray()
                        //)
                        //.SetIsOriginAllowedToAllowWildcardSubdomains()
                        .SetIsOriginAllowed(ori => true)
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

            //是否启用HTTP严格传输安全协议(HSTS)
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                options.ExcludedHosts.Add("example.com");
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
              .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources(Configuration))
              .AddInMemoryApiResources(IdentityServerConfig.GetApiResources(Configuration))
              .AddInMemoryClients(IdentityServerConfig.GetClients(Configuration))
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
       x     */
        }

        /// <summary>
        /// Autofac执行注入的地方 ConfigureServices之后执行
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacDIExtensionsModule());
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation("Begin Startup Configure......");
            if (env.IsDevelopment())
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
                });// this will add the global exception handle for production evironment.
                app.UseHsts();
            }

            app.UseCors(LocalCorsPolicyName); //Enable CORS!

            //开启HTTPS重定向
            app.UseHttpsRedirection();

            //授权相关：服务端代码
            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseRouting();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
