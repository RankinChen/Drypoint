using Drypoint.Unity;
using Drypoint.Unity.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [ApiController]
    public class ApplicationService : IApplicationService
    {
        /// <summary>
        /// 
        /// </summary>
       
        public static string[] CommonPostfixes = { "AppService", "ApplicationService" };

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            
            //TODO

        }
    }
}
