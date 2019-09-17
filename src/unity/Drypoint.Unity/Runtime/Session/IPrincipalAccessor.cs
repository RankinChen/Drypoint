using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Drypoint.Unity.Runtime.Session
{
    /// <summary>
    /// 票据集合
    /// </summary>
    public interface IPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; }
    }
}
