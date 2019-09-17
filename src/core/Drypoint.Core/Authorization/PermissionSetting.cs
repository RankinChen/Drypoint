using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drypoint.Core.Common.Auditing;

namespace Drypoint.Core.Authorization
{
    [Table("DrypointPermissions")]
    public class PermissionSetting : CreationAuditedEntity<long>
    {

        [Required]
        [StringLength(128)]
        public virtual string Name { get; set; }

        public virtual bool IsGranted { get; set; }

        protected PermissionSetting()
        {
            IsGranted = true;
        }
    }
}
