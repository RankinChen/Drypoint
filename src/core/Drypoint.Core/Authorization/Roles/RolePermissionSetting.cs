using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Core.Authorization.Roles
{
    public class RolePermissionSetting : PermissionSetting
    {
        public virtual long RoleId { get; set; }
    }
}
