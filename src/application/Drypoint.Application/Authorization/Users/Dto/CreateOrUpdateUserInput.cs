using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Drypoint.Application.Authorization.Users.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateOrUpdateUserInput
    {/// <summary>
    /// 
    /// </summary>
        [Required]
        public UserEditDto User { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string[] AssignedRoleNames { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool SendActivationEmail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool SetRandomPassword { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<long> OrganizationUnits { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CreateOrUpdateUserInput()
        {
            OrganizationUnits = new List<long>();
        }
    }
}