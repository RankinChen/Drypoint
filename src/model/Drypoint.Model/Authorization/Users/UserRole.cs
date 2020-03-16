using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drypoint.Model.Common.Auditing;
using Drypoint.Model.Authorization.Roles;

namespace Drypoint.Model.Authorization.Users
{
    [Table("DrypointUserRoles")]
    public class UserRole : CreationAuditedEntity<long>
    {
        public virtual long UserId { get; set; }
        public virtual long RoleId { get; set; }

        public UserRole()
        {

        }
        public UserRole(long userId, int roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}
