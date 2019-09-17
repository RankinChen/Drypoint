using Drypoint.Unity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Drypoint.Host.Core.Authentication.JwtBearer
{
    public class JwtSecurityTokenValidator : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtSecurityTokenValidator()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters,
            out SecurityToken validatedToken)
        {
            //var cacheManager = IocManager.Instance.Resolve<ICacheManager>();

            //var principal = _tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);

            //var userIdentifierString = principal.Claims.First(c => c.Type == DrypointConsts.CacheKey_UserIdentifier);
            //var tokenValidityKeyInClaims = principal.Claims.First(c => c.Type == DrypointConsts.CacheKey_TokenValidityKey);

            //var tokenValidityKeyInCache = cacheManager
            //    .GetCache(DrypointConsts.CacheKey_TokenValidityKey)
            //    .GetOrDefault(tokenValidityKeyInClaims.Value);

            //if (tokenValidityKeyInCache != null) return principal;

            //if (long.TryParse(userIdentifierString.Value, out long userIdentifier))
            //{
            //    var userManagerObject = userManager.Object;

            //    var user = userManagerObject.GetUser(userIdentifier);
            //    var isValidityKetValid = userManagerObject.IsTokenValidityKeyValidAsync(user, tokenValidityKeyInClaims.Value).Result;

            //    if (isValidityKetValid)
            //    {
            //        cacheManager
            //            .GetCache(DrypointConsts.CacheKey_TokenValidityKey)
            //            .Set(tokenValidityKeyInClaims.Value, "");

            //        return principal;
            //    }

            //}
            throw new SecurityTokenException("invalid");
        }
    }
}
