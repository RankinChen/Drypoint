using Drypoint.Unity.Dependency;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Drypoint.Unity.Runtime.Session
{
    public class ClaimsSession : IDrypointSession, ISingletonDependency
    {
        protected IPrincipalAccessor PrincipalAccessor { get; }

        public ClaimsSession(IPrincipalAccessor principalAccessor)

        {
            PrincipalAccessor = principalAccessor;
        }


        public long? UserId
        {
            get
            {

                //var userIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
                var userIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);

                if (string.IsNullOrEmpty(userIdClaim?.Value))
                {
                    return null;
                }

                long userId;
                if (!long.TryParse(userIdClaim.Value, out userId))
                {
                    return null;
                }

                return userId;
            }
        }
        public long? RoleId
        {
            get
            {
                var userIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Role);
                if (string.IsNullOrEmpty(userIdClaim?.Value))
                {
                    return null;
                }

                long userId;
                if (!long.TryParse(userIdClaim.Value, out userId))
                {
                    return null;
                }

                return userId;
            }
        }
    }
}
