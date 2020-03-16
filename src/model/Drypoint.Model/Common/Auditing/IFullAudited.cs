using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Model.Common.Auditing
{
    public interface IFullAudited : IAudited, IDeletionAudited
    {

    }

    public interface IFullAudited<TUser> : IAudited<TUser>, IFullAudited, IDeletionAudited<TUser>
    where TUser : IEntity<long>
    {

    }
}
