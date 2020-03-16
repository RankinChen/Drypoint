using Drypoint.Model.Common;
using Drypoint.Model.Common.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Drypoint.Model.Authorization.Users
{
    [Table("DrypointUserLoginAttempts")]
    public class UserLoginAttempt : Entity<long>, ICreationAudited
    {

        /// <summary>
        /// User's Id, if <see cref="UserNameOrEmailAddress"/> was a valid username or email address.
        /// </summary>
        public virtual long? UserId { get; set; }

        [StringLength(255)]
        public virtual string UserNameOrEmailAddress { get; set; }

        [StringLength(64)]
        public virtual string ClientIpAddress { get; set; }

        [StringLength(128)]
        public virtual string ClientName { get; set; }

        [StringLength(512)]
        public virtual string BrowserInfo { get; set; }

        public virtual LoginResultType Result { get; set; }

        public long? CreatorUserId { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public UserLoginAttempt()
        {
            CreationTime = DateTime.Now;
        }
    }
}
