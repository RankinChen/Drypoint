using Drypoint.Unity.Dependency;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Application.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class DrypointAuthorizeAttribute : Attribute
    {
        public string[] Permissions { get; }

        /// <summary>
        /// 如果为true 则所有权限都满足才能访问，
        /// 如果为false 则满足个权限即可
        /// 默认为false
        /// </summary>
        public bool RequireAllPermissions { get; set; }

        public DrypointAuthorizeAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }
    }
}
