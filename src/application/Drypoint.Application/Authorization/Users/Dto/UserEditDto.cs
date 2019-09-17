using System.ComponentModel.DataAnnotations;

namespace Drypoint.Application.Authorization.Users.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class UserEditDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Surname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ShouldChangePasswordOnNextLogin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsTwoFactorEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsLockoutEnabled { get; set; }

    }
}