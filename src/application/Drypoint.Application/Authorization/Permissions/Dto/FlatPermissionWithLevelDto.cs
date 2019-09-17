using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Application.Authorization.Permissions.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class FlatPermissionWithLevelDto : FlatPermissionDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Level { get; set; }
    }
}
