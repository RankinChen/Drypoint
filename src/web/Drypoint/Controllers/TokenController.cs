using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Drypoint.Models;
using Drypoint.Unity;
using Drypoint.Unity.OptionsConfigModels;
using Drypoint.Unity.Runtime.Session;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Drypoint.Controllers
{
    [ApiExplorerSettings(GroupName = DrypointConsts.AppAPIGroupName)]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AuthManagement _authManagement;
        private readonly IDrypointSession _drypointSession;

        public TokenController(IConfiguration configuration,
            IOptions<AuthManagement> authManagement,
            IDrypointSession drypointSession,
            IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _authManagement = authManagement.Value;
            _drypointSession = drypointSession;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(LoginModel login)
        {
            IActionResult response = Unauthorized();
            var user = Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns></returns>
        private string BuildToken(UserModel user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(JwtClaimTypes.Subject, "123456"));
            claims.Add(new Claim(JwtClaimTypes.Name, user.Name));
            claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
            claims.Add(new Claim(JwtClaimTypes.BirthDate, user.Birthdate.ToString("yyyy-MM-dd")));

            //attach roles
            foreach (string role in user.Roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }
               

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authManagement.JwtBearer.SecurityKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               _authManagement.JwtBearer.Issuer,
               _authManagement.JwtBearer.Audience,
               claims,
              expires: DateTime.Now.AddMinutes(_authManagement.JwtBearer.AccessExpiration), //过期时间
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //Authenticates login information, retrieves authorization infomation (roles)
        private UserModel Authenticate(LoginModel login)
        {
            UserModel user = new UserModel
            {
                Name = "admin",
                Birthdate = DateTime.Now.AddYears(-30),
                Email = "test@test.com",
                Roles = new string[] { "admin", "test" }
            };
            return user;
        }

        [HttpGet("Logout")]
        public void Logout()
        {
            //if (_drypointSession.UserId != null)
            //{
            //    var tokenValidityKey = User.Claims.First(c => c.Type == JwtClaimTypes.Id);
            //}
        }
    }
}