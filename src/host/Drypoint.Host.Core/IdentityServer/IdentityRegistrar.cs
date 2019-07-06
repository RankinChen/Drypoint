using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Drypoint.Core.Authorization.Users;

namespace Drypoint.Host.Core.Identity
{
    public static class IdentityRegistrar
    {
        public static IdentityBuilder Register(IServiceCollection services)
        {
            services.AddLogging();
            return services.AddIdentityCore<UserBase>(options =>
             {

             });
            //.AddUserManager<UserManager>()
            //.AddRoleManager<RoleManager>()
            //.AddUserStore<UserStore>()
            //.AddRoleStore<RoleStore>()
            //.AddUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
            //.AddSecurityStampValidator<SecurityStampValidator>()
            //.AddPermissionChecker<PermissionChecker>()
            //.AddDefaultTokenProviders();
        }
    }
}
