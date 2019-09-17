using Drypoint.Application.Authorization.Permissions.Dto;
using System.Collections.Generic;

namespace Drypoint.Application.Authorization.Roles.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class GetRoleForEditOutput
    {/// <summary>
     /// 
     /// </summary>
        public RoleEditDto Role { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FlatPermissionDto> Permissions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> GrantedPermissionNames { get; set; }
    }
}