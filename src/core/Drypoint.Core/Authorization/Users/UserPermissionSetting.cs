using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Core.Authorization.Users
{
    public class UserPermissionSetting : PermissionSetting
    {
        public virtual long UserId { get; set; }
    }
}
