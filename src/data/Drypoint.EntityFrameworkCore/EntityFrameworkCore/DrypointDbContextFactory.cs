using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Drypoint.Unity.Configuration;
using Drypoint.Unity;

namespace Drypoint.EntityFrameworkCore.EntityFrameworkCore
{
    public class DrypointDbContextFactory : IDesignTimeDbContextFactory<DrypointDbContext>
    {
        public DrypointDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DrypointDbContext>();

            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder(), addUserSecrets: true);
            
            DrypointDbContextConfigurer.Configure(builder, configuration.GetConnectionString(DrypointConsts.ConnectionStringName_PostgreSQL),DBCategory.PostgreSQL);

            return new DrypointDbContext(builder.Options);
        }
    }
}
