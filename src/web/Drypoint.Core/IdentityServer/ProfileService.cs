using Drypoint.Model.Authorization.Users;
using Drypoint.EntityFrameworkCore.Repositories;
using Drypoint.Unity.Dependency;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Drypoint.Core.IdentityServer
{
    public class ProfileService : IProfileService, ITransientDependency
    {
        private readonly ILogger _logger;
        private readonly IRepository<User, long> _userRepository;
        public ProfileService(
                ILogger<ProfileService> logger,
                IRepository<User, long> userRepository)
        {

            _logger = logger;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 只要有关用户的身份信息单元被请求（例如在令牌创建期间或通过用户信息终点），就会调用此方法
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.LogProfileRequest(_logger);

            
            //var claims = context.Subject.Claims.ToList();
            //context.IssuedClaims = claims.ToList();

            //判断是否有请求Claim信息
            if (context.RequestedClaimTypes.Any())
            {
                //根据用户唯一标识查找用户信息
                var user = _userRepository.GetAllIncluding(aa => aa.Claims).FirstOrDefault(aa => aa.Id == Convert.ToInt64(context.Subject.GetSubjectId()));
                if (user != null)
                {
                    //调用此方法以后内部会进行过滤，只将用户请求的Claim加入到 context.IssuedClaims 集合中 这样我们的请求方便能正常获取到所需Claim
                    var claims = user.Claims.Select(aa => new Claim(aa.ClaimType, aa.ClaimValue));
                    context.AddRequestedClaims(claims);
                }
            }

            context.LogIssuedClaims(_logger);

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogDebug("IsActive called from: {caller}", context.Caller);

            var user = _userRepository.GetAllIncluding(aa => aa.Claims).FirstOrDefault(aa => aa.Id == Convert.ToInt64(context.Subject.GetSubjectId()));
            //context.IsActive = user?.IsActive == true;
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
