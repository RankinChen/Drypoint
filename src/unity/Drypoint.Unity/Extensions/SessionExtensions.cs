using Drypoint.Unity.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.Extensions
{
    public static class SessionExtensions
    {

        public static long GetUserId(this IDrypointSession session)
        {
            if (!session.UserId.HasValue)
            {
                throw new Exception("会话不存在UserId,用户可能未登陆");
            }

            return session.UserId.Value;
        }

        public static long GetRoleId(this IDrypointSession session)
        {
            if (!session.RoleId.HasValue)
            {
                throw new Exception("会话不存在RoleId,用户可能未登陆或未分配角色");
            }

            return session.RoleId.Value;
        }

        public static UserIdentifier ToUserIdentifier(this IDrypointSession session)
        {
            return session.UserId == null ? null : new UserIdentifier(session.RoleId, session.GetUserId());
        }
    }
}
