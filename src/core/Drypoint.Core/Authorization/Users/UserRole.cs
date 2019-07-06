using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drypoint.Core.Common.Auditing;

namespace Drypoint.Core.Authorization.Users
{
    [Table("DrypointUserRoles")]
    public class UserRole : CreationAuditedEntity<long>
    {
        public virtual long UserId { get; set; }

        public virtual int RoleId { get; set; }

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
