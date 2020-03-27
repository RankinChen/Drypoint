using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.OptionsConfigModels
{
    /// <summary>
    /// 认证授权配置，从appsettings.json读取
    /// </summary>

    public class AuthManagement
    {
        public IdentityServer IdentityServer { get; set; }

        public JwtBearer JwtBearer { get; set; }

        public AuthManagement()
        {
            IdentityServer = new IdentityServer();
            JwtBearer = new JwtBearer();
        }
    }

    public class IdentityServer
    {
        [JsonProperty("IsEnabled")]
        public bool IsEnabled { get; set; }
        [JsonProperty("Authority")]
        public string Authority { get; set; }
        [JsonProperty("ApiName")]
        public string ApiName { get; set; }
        [JsonProperty("ApiSecret")]
        public string ApiSecret { get; set; }

    }

    public class JwtBearer
    {
        [JsonProperty("IsEnabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("SecurityKey")]
        public string SecurityKey { get; set; }

        [JsonProperty("Issuer")]
        public string Issuer { get; set; }

        [JsonProperty("Audience")]
        public string Audience { get; set; }

        [JsonProperty("AccessExpiration")]
        public int AccessExpiration { get; set; }

        [JsonProperty("RefreshExpiration")]
        public int RefreshExpiration { get; set; }
    }
}
