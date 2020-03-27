using Drypoint.Unity;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drypoint.Core.Configuration
{
    public static class SwaggerDocExtensions
    {
        public static void AddCustomSwaggerGen(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("Authentication:IdentityServer:IsEnabled", false))
            {
                IdentityServerSecurity(services, configuration);
            }
            else
            {
                JwtBearerSecurity(services, configuration);
            }
        }

        /// <summary>
        /// IdentityServer4 使用
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void IdentityServerSecurity(IServiceCollection services, IConfiguration configuration)
        {
            //注册Swagger
            services.AddSwaggerDocument(config =>
            {
                config.AddSecurity("bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Name = "授权",
                    Description = "尝试获取授权",
                    Flows = new OpenApiOAuthFlows()
                    {
                        AuthorizationCode = new OpenApiOAuthFlow()
                        {
                            Scopes = new Dictionary<string, string>
                            {
                                { "Drypoint_Host_API", "主要API" },
                                { "openid", "openid" },
                                { "profile", "profile" },
                                { "email", "Email" },
                                { "phone", "Phone" },
                            },
                            AuthorizationUrl = $"{configuration["IdentityServer:Authority"]}/connect/authorize",
                            TokenUrl = $"{configuration["IdentityServer:Authority"]}/connect/token"
                        },
                    }
                });
                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
                config.PostProcess = document =>
                {
                    document.Info.Version = configuration["SwaggerDoc:Version"];
                    document.Info.Title = configuration["SwaggerDoc:Title"];
                    document.Info.Description = configuration["SwaggerDoc:Description"];
                    document.Info.TermsOfService = configuration["SwaggerDoc:TermsOfService"];
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = configuration["SwaggerDoc:Contact:Name"],
                        Email = configuration["SwaggerDoc:Contact:Email"],
                        Url = configuration["SwaggerDoc:Contact:Url"]
                    };
                    document.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = configuration["SwaggerDoc:License:Name"],
                        Url = configuration["SwaggerDoc:License:Url"]
                    };
                };
                config.DocumentName = "App";
                config.ApiGroupNames = new[] { DrypointConsts.AppAPIGroupName };
            })
            .AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = configuration["SwaggerDoc:Version"];
                    document.Info.Title = configuration["SwaggerDoc:Title"];
                    document.Info.Description = configuration["SwaggerDoc:Description"];
                    document.Info.TermsOfService = configuration["SwaggerDoc:TermsOfService"];
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = configuration["SwaggerDoc:Contact:Name"],
                        Email = configuration["SwaggerDoc:Contact:Email"],
                        Url = configuration["SwaggerDoc:Contact:Url"]
                    };
                    document.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = configuration["SwaggerDoc:License:Name"],
                        Url = configuration["SwaggerDoc:License:Url"]
                    };
                };
                config.DocumentName = "Admin";
                config.ApiGroupNames = new[] { DrypointConsts.AdminAPIGroupName };
            });
        }

        /// <summary>
        /// JwtBearer 使用
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void JwtBearerSecurity(IServiceCollection services, IConfiguration configuration)
        {
            //注册Swagger
            services.AddSwaggerDocument(config =>
            {
                config.AddSecurity("bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "授权",
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）",
                    In = OpenApiSecurityApiKeyLocation.Header,
                });
                config.PostProcess = document =>
                {
                    document.Info.Version = configuration["SwaggerDoc:Version"];
                    document.Info.Title = configuration["SwaggerDoc:Title"];
                    document.Info.Description = configuration["SwaggerDoc:Description"];
                    document.Info.TermsOfService = configuration["SwaggerDoc:TermsOfService"];
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = configuration["SwaggerDoc:Contact:Name"],
                        Email = configuration["SwaggerDoc:Contact:Email"],
                        Url = configuration["SwaggerDoc:Contact:Url"]
                    };
                    document.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = configuration["SwaggerDoc:License:Name"],
                        Url = configuration["SwaggerDoc:License:Url"]
                    };
                };
                config.DocumentName = "App";
                config.ApiGroupNames = new[] { DrypointConsts.AppAPIGroupName };
            })
            .AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = configuration["SwaggerDoc:Version"];
                    document.Info.Title = configuration["SwaggerDoc:Title"];
                    document.Info.Description = configuration["SwaggerDoc:Description"];
                    document.Info.TermsOfService = configuration["SwaggerDoc:TermsOfService"];
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = configuration["SwaggerDoc:Contact:Name"],
                        Email = configuration["SwaggerDoc:Contact:Email"],
                        Url = configuration["SwaggerDoc:Contact:Url"]
                    };
                    document.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = configuration["SwaggerDoc:License:Name"],
                        Url = configuration["SwaggerDoc:License:Url"]
                    };
                };
                config.DocumentName = "Admin";
                config.ApiGroupNames = new[] { DrypointConsts.AdminAPIGroupName };
            });
        }



        /// <summary>
        /// 启用自定义API文档
        /// </summary>
        public static void UseCustomSwaggerUI(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3(config =>
             {
                 config.OAuth2Client = new OAuth2ClientSettings
                 {
                     ClientId = configuration["IdentityServer:Client:ClientId"],
                     ClientSecret = configuration["IdentityServer:Client:ClientSecret"],
                     AppName = "API 端测试授权",
                     //在授权认证的时候地址栏后面添加请求参数 
                     /*
                     AdditionalQueryStringParameters = {
                         {"response_type","code id_token" },
                         { "redirect_uri","http://localhost:7000/signin-oidc"}
                     }
                     */
                 };
             });
        }
    }
}
