using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Drypoint.MVC.Auths;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Drypoint.MVC
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddControllersWithViews();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.AccessDeniedPath = "/Authorization/AccessDenied";
            }).AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = Configuration["IdentityServer:Authority"];
                options.RequireHttpsMetadata = false;

                options.ClientId = Configuration["IdentityServer:ClientId"];
                options.ClientSecret = Configuration["IdentityServer:ClientSecret"];
                options.SaveTokens = true; //取得的token持久化到Cookie
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                //options.ResponseType = OpenIdConnectResponseType.CodeIdTokenToken; //既获取id_token 又获取access_token

                options.Scope.Clear();
                options.Scope.Add("Drypoint_Host_API");
                options.Scope.Add(OidcConstants.StandardScopes.OpenId);
                options.Scope.Add(OidcConstants.StandardScopes.Profile);
                options.Scope.Add(OidcConstants.StandardScopes.Email);
                options.Scope.Add(OidcConstants.StandardScopes.Phone);

                //options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);

                //// 集合里的东西 都是要被过滤掉的属性，nbf amr exp...
                //options.ClaimActions.Remove("nbf");
                //options.ClaimActions.Remove("amr");
                //options.ClaimActions.Remove("exp");

                //// 不映射到User Claims里
                //options.ClaimActions.DeleteClaim("sid");
                //options.ClaimActions.DeleteClaim("sub");
                //options.ClaimActions.DeleteClaim("idp");

                //// 让Claim里面的角色成为mvc系统识别的角色
                //options.TokenValidationParameters = new TokenValidationParameters
                //{
                //    NameClaimType = JwtClaimTypes.Name,
                //    RoleClaimType = JwtClaimTypes.Role
                //};

                options.Events = new OpenIdConnectEvents()
                {
                    //授权被用户拒绝之后友好提示页面
                    OnRemoteFailure = context =>
                    {
                        //跳到错误指示页面
                        context.Response.Redirect("/");
                        context.HandleResponse();
                        return Task.FromResult(0);
                    }
                };
            });

            services.AddSingleton<IAuthorizationRequirement, SmithInSomewareRequirement>();
            services.AddSingleton<IAuthorizationHandler, SmithInSomewhereHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SmithInSomewhere", builder =>
                {
                    builder.AddRequirements(new SmithInSomewareRequirement());
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthentication();
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
