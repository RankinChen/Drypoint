using Drypoint.Core.Authentication.JwtBearer;
using Drypoint.Unity;
using Drypoint.Unity.OptionsConfigModels;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace Drypoint.Core.Authentication
{
    public static class AuthConfigurer
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var authManagement = configuration.GetSection("Authentication").Get<AuthManagement>();

            //使用IdentityServer
            if (authManagement.IdentityServer.IsEnabled)
            {
                //授权相关:资源端代码
                IdentityModelEventSource.ShowPII = true;
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                //客户端设置 AccessTokenType为JWT(默认)写法
                //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //        .AddJwtBearer(options =>
                //         {
                //             options.Authority = authManagement.IdentityServer.Authority;;
                //             options.RequireHttpsMetadata = false;
                //             options.Audience = authManagement.IdentityServer.ApiName;
                //         });
                //客户端设置 AccessTokenType为Reference时需要API提供认证身份认证
                services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                //资源端
                .AddIdentityServerAuthentication(options =>
                {
                    //options.JwtValidationClockSkew = TimeSpan.Zero;
                    options.Authority = authManagement.IdentityServer.Authority;
                    options.ApiName = authManagement.IdentityServer.ApiName;
                    options.ApiSecret = authManagement.IdentityServer.ApiSecret;
                    options.RequireHttpsMetadata = false;
                    options.JwtValidationClockSkew = TimeSpan.FromSeconds(0);  //验证token间隔时间
                    //待测试
                    //options.JwtBearerEvents = new JwtBearerEvents
                    //{
                    //    OnMessageReceived = QueryStringTokenResolver
                    //};
                });
            }
            else
            {
                //使用JWT 
                //从JwtClaimTypes的常量键 变成了ClaimTypes的常量键 添加这一行处理
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                services.AddAuthentication(config =>
                {
                    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    //是否需要HTTPS
                    options.RequireHttpsMetadata = false;
                    //保存Token
                    options.SaveToken = true;
                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new JwtSecurityTokenValidator());

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RoleClaimType= JwtClaimTypes.Role,
                        NameClaimType=JwtClaimTypes.Name,

                        ValidateIssuer = true,
                        ValidIssuer = authManagement.JwtBearer.Issuer,
                        ValidateAudience = false,
                        ValidAudience = authManagement.JwtBearer.Audience,

                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authManagement.JwtBearer.SecurityKey)),
                        ClockSkew = TimeSpan.Zero,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = JWTMessageReceived,
                        //OnTokenValidated
                    };
                });
            }
        }


        /* TODO 代码待测试 */
        private static Task JWTMessageReceived(MessageReceivedContext context)
        {
            if (!context.HttpContext.Request.Path.HasValue ||
                !context.HttpContext.Request.Path.Value.StartsWith("/signalr"))
                return Task.CompletedTask;

            var qsAuthToken = context.HttpContext.Request.Query["enc_auth_token"].FirstOrDefault();
            if (qsAuthToken == null)
                return Task.CompletedTask;

            context.Token = SimpleStringCipher.Instance.Decrypt(qsAuthToken, DrypointConsts.DefaultPassPhrase);
            return Task.CompletedTask;
        }
    }
}
