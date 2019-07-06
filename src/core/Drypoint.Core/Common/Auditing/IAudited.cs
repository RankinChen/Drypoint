using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Core.Common.Auditing
{
    public interface IAudited : ICreationAudited, IModificationAudited
    {

       
    }

    public interface IAudited<TUser> : IAudited, ICreationAudited<TUser>, IModificationAudited<TUser>
        where TUser : IEntity<long>
    {

    }
}
