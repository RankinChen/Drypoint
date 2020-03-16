using Drypoint.Model.Common.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Drypoint.Model.Authorization.Users
{
    [Table("DrypointUserAccounts")]
    public class UserAccount : FullAuditedEntity<long>
    {
        public virtual long UserId { get; set; }

        public virtual long? UserLinkId { get; set; }

        [StringLength(256)]
        public virtual string UserName { get; set; }

        [StringLength(256)]
        public virtual string EmailAddress { get; set; }
    }
}
