using System;
using System.Collections.Generic;

namespace Drypoint.Application.Authorization.Users.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class GetUserForEditOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public UserEditDto User { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public UserRoleDto[] Roles { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> MemberedOrganizationUnits { get; set; }
    }
}