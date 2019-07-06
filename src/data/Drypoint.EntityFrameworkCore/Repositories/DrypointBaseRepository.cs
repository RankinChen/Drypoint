using Drypoint.EntityFrameworkCore.Repositories;
using Drypoint.Core.Common;

namespace Drypoint.EntityFrameworkCore.EntityFrameworkCore.Repositories
{
    public class DrypointBaseRepository<TEntity, TPrimaryKey> : EfBaseRepository<DrypointDbContext,TEntity,TPrimaryKey> 
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public DrypointBaseRepository(IDbContextProvider<DrypointDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
