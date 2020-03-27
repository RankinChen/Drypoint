using Drypoint.Application.Authorization;
using Drypoint.Application.Authorization.Permissions;
using Drypoint.Unity.Dependency;
using Drypoint.Unity.Runtime.Session;
using DrypointException;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Drypoint.Core.Authorization
{
    public class AuthorizationHelper : IAuthorizationHelper, ITransientDependency
    {
        public IDrypointSession _drypointSession { get; set; }
        public IPermissionChecker _permissionChecker { get; set; }

        public AuthorizationHelper(
            IDrypointSession drypointSession,
            IPermissionChecker permissionChecker)
        {
            _drypointSession = drypointSession;
            _permissionChecker = permissionChecker;
        }

        public async Task AuthorizeAsync(IEnumerable<DrypointAuthorizeAttribute> authorizeAttributes)
        {
            if (!_drypointSession.UserId.HasValue)
            {
                throw new AuthorizationException("用户未登陆！");
            }

            foreach (var authorizeAttribute in authorizeAttributes)
            {
               await _permissionChecker.AuthorizeAsync(authorizeAttribute.RequireAllPermissions, authorizeAttribute.Permissions);
            }
        }

        public async Task AuthorizeAsync(MethodInfo methodInfo, Type type)
        {
            await CheckPermissions(methodInfo, type);
        }


        protected virtual async Task CheckPermissions(MethodInfo methodInfo, Type type)
        {

            if (AllowAnonymous(methodInfo, type))
            {
                return;
            }

            if (IsPropertyGetterSetterMethod(methodInfo, type))
            {
                return;
            }

            if (!methodInfo.IsPublic && !methodInfo.GetCustomAttributes().OfType<DrypointAuthorizeAttribute>().Any())
            {
                return;
            }

            var authorizeAttributes =GetAttributesOfMemberAndType(methodInfo, type).OfType<DrypointAuthorizeAttribute>().ToArray();

            if (!authorizeAttributes.Any())
            {
                return;
            }

            await AuthorizeAsync(authorizeAttributes);
        }

        private static bool AllowAnonymous(MemberInfo memberInfo, Type type)
        {
            return GetAttributesOfMemberAndType(memberInfo,type).OfType<AllowAnonymousAttribute>().Any();
        }

       private static bool IsPropertyGetterSetterMethod(MethodInfo method, Type type)
        {
            if (!method.IsSpecialName)
            {
                return false;
            }

            if (method.Name.Length < 5)
            {
                return false;
            }

            return type.GetProperty(method.Name.Substring(4), BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic) != null;
        }

        public static List<object> GetAttributesOfMemberAndType(MemberInfo memberInfo, Type type, bool inherit = true)
        {
            var attributeList = new List<object>();
            attributeList.AddRange(memberInfo.GetCustomAttributes(inherit));
            attributeList.AddRange(type.GetTypeInfo().GetCustomAttributes(inherit));
            return attributeList;
        }

    }
}
