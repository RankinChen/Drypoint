using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Model.Common.Auditing
{ 
    /// <summary>
    /// 包含删除操作用户的Id 和删除时间
    /// </summary>
public interface IDeletionAudited
    {
        /// <summary>
        /// 删除用户Id
        /// </summary>
        long? DeleterUserId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        DateTime? DeletionTime { get; set; }

    }

    public interface IDeletionAudited<TUser> : IDeletionAudited
        where TUser : IEntity<long>
    {
        TUser DeleterUser { get; set; }
    }
}
