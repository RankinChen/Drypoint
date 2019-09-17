using Drypoint.Unity.Dependency;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace Drypoint.Unity.Runtime.Session
{
    /// <summary>
    /// 获取票据集合
    /// </summary>
    public class AspNetCorePrincipalAccessor : IPrincipalAccessor, ISingletonDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCorePrincipalAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public ClaimsPrincipal Principal => _httpContextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;

    }
}
