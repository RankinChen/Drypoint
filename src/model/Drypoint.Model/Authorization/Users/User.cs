using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Drypoint.Model.Common.Auditing;
using Drypoint.Model.Common;
using Drypoint.Unity;
using Drypoint.Model.Configuration;
using Drypoint.Unity.Extensions;

namespace Drypoint.Model.Authorization.Users
{
    [Table("DrypointUser")]
    public class User : FullAuditedEntity<long>, IPassivable
    {
        [StringLength(64)]
        public virtual string AuthenticationSource { get; set; }

        [Required]
        [StringLength(256)]
        public virtual string UserName { get; set; }

        [Required]
        [StringLength(256)]
        public virtual string EmailAddress { get; set; }

        [Required]
        [StringLength(64)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(64)]
        public virtual string Surname { get; set; }

        [NotMapped]
        public virtual string FullName { get { return this.Name + " " + this.Surname; } }

        [Required]
        [StringLength(128)]
        public virtual string Password { get; set; }

        [StringLength(328)]
        public virtual string EmailConfirmationCode { get; set; }

        [StringLength(328)]
        public virtual string PasswordResetCode { get; set; }

        public virtual DateTime? LockoutEndDateUtc { get; set; }

        public virtual int AccessFailedCount { get; set; }

        public virtual bool IsLockoutEnabled { get; set; }

        [StringLength(32)]
        public virtual string PhoneNumber { get; set; }

        public virtual bool IsPhoneNumberConfirmed { get; set; }

        [StringLength(128)]
        public virtual string SecurityStamp { get; set; }

        public virtual bool IsTwoFactorEnabled { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<UserLogin> Logins { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<UserRole> Roles { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<UserClaim> Claims { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<UserPermissionSetting> Permissions { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<Setting> Settings { get; set; }

        public virtual bool IsEmailConfirmed { get; set; }

        public virtual bool IsActive { get; set; }

        protected User()
        {
            IsActive = true;
            SecurityStamp = Guid.NewGuid().ToString();
        }

        public virtual void SetNewPasswordResetCode()
        {
            PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(328);
        }

        public virtual void SetNewEmailConfirmationCode()
        {
            EmailConfirmationCode = Guid.NewGuid().ToString("N").Truncate(328);
        }

        public override string ToString()
        {
            return $"[User {Id}] {UserName}";
        }
    }
}
