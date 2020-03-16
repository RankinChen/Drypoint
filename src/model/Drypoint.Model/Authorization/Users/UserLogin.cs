using System;
using System.Collections.Generic;
using System.Text;
using Drypoint.Model.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Drypoint.Model.Authorization.Users
{
    [Table("DrypointUserLogins")]
    public class UserLogin : Entity<long>
    {
        public virtual long UserId { get; set; }

        [Required]
        [StringLength(128)]
        public virtual string LoginProvider { get; set; }

        [Required]
        [StringLength(256)]
        public virtual string ProviderKey { get; set; }

        public UserLogin()
        {

        }

        public UserLogin(long userId, string loginProvider, string providerKey)
        {
            UserId = userId;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }
    }
}
