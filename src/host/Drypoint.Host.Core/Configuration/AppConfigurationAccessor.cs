using Drypoint.Host.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace Drypoint.Host.Core.Configuration
{
    public class AppConfigurationAccessor : IAppConfigurationAccessor
    {
        public IConfigurationRoot Configuration { get; }

        public AppConfigurationAccessor(IHostingEnvironment env)
        {
            Configuration = env.GetAppConfiguration();
        }
    }
}
