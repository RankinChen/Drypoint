using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Model.Common.Auditing
{    
    /// <summary>
     /// 包含更新操作用户的Id 和更新时间
     /// </summary>
    public interface IModificationAudited
    {
        /// <summary>
        /// 更新用户Id
        /// </summary>
        long? LastModifierUserId { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        DateTime? LastModificationTime { get; set; }
    }

    public interface IModificationAudited<TUser> : IModificationAudited
        where TUser : IEntity<long>
    {
        TUser LastModifierUser { get; set; }
    }
}
