using Drypoint.Model.Authorization.Users;
using Drypoint.EntityFrameworkCore.Repositories;
using Drypoint.Unity.Dependency;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Drypoint.Core.IdentityServer
{
    /// <summary>
    /// 用户名密码验证方式时生效
    /// </summary>
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator, ITransientDependency
    {
        private readonly ILogger _logger;
        private readonly IRepository<User, long> _userRepository;
        private readonly ISystemClock _clock;

        public ResourceOwnerPasswordValidator(
            ILogger<ResourceOwnerPasswordValidator> logger,
            IRepository<User, long> userRepository,
            ISystemClock clock)
        {

            _logger = logger;
            _userRepository = userRepository;
            _clock = clock;
        }
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //此处使用context.UserName, context.Password 用户名和密码来与数据库的数据做校验
            var user = _userRepository.GetAllIncluding(aa => aa.Claims).FirstOrDefault(aa => (aa.Name == context.UserName || aa.UserName == context.UserName) && aa.Password == context.Password);
            if (user != null)
            {
                //验证通过返回结果 
                //subjectId 为用户唯一标识 一般为用户id
                //authenticationMethod 描述自定义授权类型的认证方法 
                //authTime 授权时间
                //claims 需要返回的用户身份信息单元 此处应该根据我们从数据库读取到的用户信息 添加Claims 
                //如果是从数据库中读取角色信息，那么我们应该在此处添加 此处只返回必要的Claim

                context.Result = new GrantValidationResult(
                    subject: user.Id.ToString(),
                    authenticationMethod: OidcConstants.AuthenticationMethods.Password,
                    authTime: _clock.UtcNow.UtcDateTime,
                    claims: GetUserClaims(user));
            }
            else
            {
                //验证失败
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 可以根据需要设置相应的Claim
        /// </summary>
        /// <returns></returns>
        private Claim[] GetUserClaims(User user)
        {
            return new Claim[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(JwtClaimTypes.Name,user.Name),
                new Claim(JwtClaimTypes.GivenName, user.UserName),
                new Claim(JwtClaimTypes.FamilyName, user.FullName),
                new Claim(JwtClaimTypes.Email,user.EmailAddress),
                new Claim(JwtClaimTypes.Role,"admin")
            };
        }
    }
}
