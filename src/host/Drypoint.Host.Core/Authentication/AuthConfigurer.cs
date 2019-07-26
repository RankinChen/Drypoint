﻿using Drypoint.Host.Core.Authentication.JwtBearer;
using Drypoint.Unity;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace Drypoint.Host.Core.Authentication
{
    public static class AuthConfigurer
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            IdentityModelEventSource.ShowPII = true;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(option =>
            {
                //option.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = ProtocolTypes.OpenIdConnect;
            })
            //访问客户端???
            .AddOpenIdConnect(ProtocolTypes.OpenIdConnect, "OpenID Connect", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                options.SaveTokens = true;

                options.Authority = configuration["IdentityServer:Authority"];
                options.ClientId = "hybrid";
            })
            //资源端
            .AddIdentityServerAuthentication(options =>
            {
                options.JwtValidationClockSkew = TimeSpan.Zero;
                options.Authority = configuration["IdentityServer:Authority"];
                options.ApiName = configuration["IdentityServer:ApiName"];
                options.ApiSecret = configuration["IdentityServer:ApiSecret"];
                options.RequireHttpsMetadata = false;
                //待测试
                options.JwtBearerEvents = new JwtBearerEvents
                {
                    OnMessageReceived = QueryStringTokenResolver
                };
            });
        }

        /* 此方法用于授权SignalR javascript客户机。
         * SignalR无法发送授权头。因此，我们将它作为加密文本从查询字符串中获取。 */
        private static Task QueryStringTokenResolver(MessageReceivedContext context)
        {
            if (!context.Request.Path.HasValue ||
                !context.Request.Path.Value.StartsWith("/signalr"))
            {
                return Task.CompletedTask;
            }

            var qsAuthToken = context.Request.Query["access_token"].FirstOrDefault();
            if (qsAuthToken == null)
            {
                return Task.CompletedTask;
            }
            context.Token = SimpleStringCipher.Instance.Decrypt(qsAuthToken, DrypointConsts.DefaultPassPhrase);
            return Task.CompletedTask;
        }
    }
}