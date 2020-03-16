using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drypoint.Model.Common.Auditing;
using System.Security.Claims;

namespace Drypoint.Model.Authorization.Users
{
    [Table("DrypointUserClaims")]
    public class UserClaim : CreationAuditedEntity<long>
    {

        public virtual long UserId { get; set; }

        [StringLength(256)]
        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }

        public UserClaim()
        {

        }

        public UserClaim(User user, Claim claim)
        {
            UserId = user.Id;
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }
}
