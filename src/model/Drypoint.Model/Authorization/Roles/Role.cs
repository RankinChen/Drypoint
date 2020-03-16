using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Drypoint.Model.Common.Auditing;
using Drypoint.Model.Common;

namespace Drypoint.Model.Authorization.Roles
{
    [Table("DrypointRole")]
    public class Role : FullAuditedEntity<long>
    {
        [Required]
        [StringLength(32)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(64)]
        public virtual string DisplayName { get; set; }

        public virtual bool IsStatic { get; set; }
        public virtual bool IsDefault { get; set; }

        [ForeignKey("RoleId")]
        public virtual ICollection<RolePermissionSetting> Permissions { get; set; }

        public Role()
        {
            Name = Guid.NewGuid().ToString("N");
        }

        public Role( string displayName)
            : this()
        {
            DisplayName = displayName;
        }

        public Role( string name, string displayName)
            : this(displayName)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"[Role {Id}, Name={Name}]";
        }
    }
}
