using System.ComponentModel.DataAnnotations;

namespace Drypoint.Application.Authorization.Roles.Dto
{/// <summary>
/// 
/// </summary>
    public class RoleEditDto
    {/// <summary>
    /// 
    /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string DisplayName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDefault { get; set; }
    }
}