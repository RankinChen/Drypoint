using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Model.Authorization.Roles
{
    public class RolePermissionSetting : PermissionSetting
    {
        public virtual long RoleId { get; set; }
    }
}
