using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Drypoint.Unity;

namespace Drypoint.EntityFrameworkCore.EntityFrameworkCore
{
    public class DrypointDbContextFactory : IDesignTimeDbContextFactory<DrypointDbContext>
    {
        public IConfiguration _configuration { get; }
        public DrypointDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DrypointDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DrypointDbContext>();

            DrypointDbContextConfigurer.Configure(builder, _configuration.GetConnectionString(DrypointConsts.ConnectionStringName_PostgreSQL), DBCategory.PostgreSQL);

            return new DrypointDbContext(builder.Options);
        }
    }
}
