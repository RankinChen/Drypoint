using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Drypoint.Host.Core.IdentityServer
{
    public static class IdentityServerConfig
    {
        /// <summary>
        /// api资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources(IConfigurationRoot configuration)
        {

            var apiResources = new List<ApiResource>();

            foreach (var child in configuration.GetSection("IdentityServer:ApiResources").GetChildren())
            {
                apiResources.Add(new ApiResource(child["ApiName"], child["DisplayName"])
                {
                    Description = child["Description"],
                    ApiSecrets = { new Secret(child["ApiSecret"].Sha256()) },
                    //请求范围
                    //Scopes = new List<Scope>
                    //{
                    //    new Scope("api.read"),
                    //    new Scope("api.write")
                    //}
                });
            }
            return apiResources;
        }

        /// <summary>
        /// 身份资源范围
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources(IConfigurationRoot configuration)
        {
            //IdentityServer支持的一些标准OpenID Connect定义的范围
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(), //必须包含
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                //自定义
                //new IdentityResource {
                //    Name = "role",
                //    UserClaims = new List<string> {"role"}
                //}
            };
        }

        /// <summary>
        /// 客户端
        /// </summary>
        public static IEnumerable<Client> GetClients(IConfigurationRoot configuration)
        {
            var clients = new List<Client>();

            foreach (var child in configuration.GetSection("IdentityServer:Clients").GetChildren())
            {
                var client = new Client
                {
                    ClientId = child["ClientId"],
                    ClientName = child["ClientName"],
                    ClientUri = child["ClientUri"] ?? "",
                    AllowedGrantTypes = child.GetSection("AllowedGrantTypes").GetChildren().Select(c => c.Value).ToArray(),
                    AllowedCorsOrigins = child.GetSection("AllowedCorsOrigins").GetChildren().Select(c => c.Value)?.ToArray(),
                    AccessTokenType = AccessTokenType.Reference,
                    ClientSecrets = child.GetSection("ClientSecrets").GetChildren().Select(secret => new Secret(secret["Value"].Sha256())).ToArray(),
                    AllowedScopes = child.GetSection("AllowedScopes").GetChildren().Select(c => c.Value).ToArray(),
                    RedirectUris = child.GetSection("RedirectUris").GetChildren().Select(c => c.Value).ToArray(),
                    PostLogoutRedirectUris = child.GetSection("PostLogoutRedirectUris").GetChildren().Select(c => c.Value).ToArray(),
                };

                if (bool.TryParse(child["AllowAccessTokensViaBrowser"], out bool allowAccessTokensViaBrowser))
                {
                    client.AllowAccessTokensViaBrowser = allowAccessTokensViaBrowser;
                }
                if (bool.TryParse(child["RequireConsent"], out bool requireConsent))
                {
                    client.RequireConsent = requireConsent;
                }
                if (bool.TryParse(child["AllowOfflineAccess"], out bool allowOfflineAccess))
                {
                    client.AllowOfflineAccess = allowOfflineAccess;
                }
                if (bool.TryParse(child["AlwaysIncludeUserClaimsInIdToken"], out bool alwaysIncludeUserClaimsInIdToken))
                {
                    client.AlwaysIncludeUserClaimsInIdToken = alwaysIncludeUserClaimsInIdToken;
                }
                

                client.AccessTokenLifetime = int.TryParse(child["AccessTokenLifetime"], out int accessTokenLifetime) ? accessTokenLifetime : 60 * 30;

                clients.Add(client);

            }

            return clients;
        }

        public static List<IdentityServer4.Test.TestUser> GetTestUser()
        {
            List<IdentityServer4.Test.TestUser> ltUser = new List<IdentityServer4.Test.TestUser>();

            ltUser.Add(new IdentityServer4.Test.TestUser
            {
                SubjectId = "1",
                Username = "admin",
                Password = "123456"
            });
            ltUser.Add(new IdentityServer4.Test.TestUser
            {
                SubjectId = "2",
                Username = "user",
                Password = "123456"
            });
            return ltUser;
        }
    }
}
