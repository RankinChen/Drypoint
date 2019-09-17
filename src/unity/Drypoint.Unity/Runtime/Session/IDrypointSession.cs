using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.Runtime.Session
{
    /// <summary>
    /// 存储当前用户相关会话
    /// </summary>
    public interface IDrypointSession
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        long? UserId { get; }

        /// <summary>
        /// 角色Id
        /// </summary>
        long? RoleId { get; }
    }
}
