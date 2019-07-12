﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Drypoint.Core.Common.Auditing;
using Drypoint.Core.Common;
using System.Security.Claims;

namespace Drypoint.Core.Authorization.Roles
{
    [Table("DrypointRoleClaims")]
    public class RoleClaim : CreationAuditedEntity<long>
    {


        public virtual int RoleId { get; set; }

        [StringLength(256)]
        public virtual string ClaimType { get; set; }

        public virtual string ClaimValue { get; set; }

        public RoleClaim()
        {

        }

        public RoleClaim(RoleBase role, Claim claim)
        {
            RoleId = role.Id;
            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }
    }
}