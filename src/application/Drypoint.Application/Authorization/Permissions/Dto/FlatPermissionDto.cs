using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Application.Authorization.Permissions.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class FlatPermissionDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string ParentName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsGrantedByDefault { get; set; }
    }
}
