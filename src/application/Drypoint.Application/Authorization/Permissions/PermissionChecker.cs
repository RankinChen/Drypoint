using Drypoint.Model.Authorization.Roles;
using Drypoint.Model.Authorization.Users;
using Drypoint.EntityFrameworkCore.Repositories;
using Drypoint.Unity.Dependency;
using Drypoint.Unity.Extensions.Collections;
using Drypoint.Unity.Runtime.Session;
using DrypointException;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drypoint.Application.Authorization.Permissions
{
    public class PermissionChecker : IPermissionChecker, ITransientDependency
    {
        private readonly ILogger _logger;
        private readonly IDrypointSession _drypointSession;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role, long> _roleRepository;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionRepository;
        private readonly IRepository<UserPermissionSetting, long> _userPermissionRepository;


        public PermissionChecker(
            ILogger<PermissionChecker> logger,
            IDrypointSession drypointSession,
            IRepository<User, long> userRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role, long> roleRepository,
            IRepository<RolePermissionSetting, long> rolePermissionRepository,
            IRepository<UserPermissionSetting, long> userPermissionRepository
            )
        {
            _logger = logger;
            _drypointSession = drypointSession;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _userPermissionRepository = userPermissionRepository;
        }

        public async Task AuthorizeAsync(bool requireAll = true, params string[] permissionNames)
        {
            if (await IsGrantedAsync(requireAll, permissionNames))
            {
                return;
            }
            /// 如果为true 则所有权限都满足才能访问，
            /// 如果为false 则满足个权限即可

            if (requireAll)
            {
                throw new AuthorizationException($"{permissionNames.JoinAsString(",")}权限未完全满足！");
            }
            else
            {
                throw new AuthorizationException($"{permissionNames.JoinAsString(",")} 权限都未满足！");
            }
        }

        public async Task<bool> IsGrantedAsync(string permissionName)
        {
            return _drypointSession.UserId.HasValue && await IsGrantedAsync(_drypointSession.UserId.Value, permissionName);
        }

        public async Task<bool> IsGrantedAsync(IUserIdentifier userIdentifier, string permissionName)
        {
            return await IsGrantedAsync(userIdentifier.UserId, permissionName);
        }

        public async Task<bool> IsGrantedAsync(bool requiresAll, params string[] permissionNames)
        {
            if (permissionNames.IsNullOrEmpty())
            {
                return true;
            }

            if (requiresAll)
            {
                foreach (var permissionName in permissionNames)
                {
                    if (!(await IsGrantedAsync(permissionName)))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                foreach (var permissionName in permissionNames)
                {
                    if (await IsGrantedAsync(permissionName))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// TODO 改成从缓存查询
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permissionName"></param>
        /// <returns></returns>
        private async Task<bool> IsGrantedAsync(long userId, string permissionName)
        {
            var userRoleIds = _userRoleRepository.GetAll().Where(aa => aa.UserId == userId).Select(aa => aa.RoleId);
            if (userRoleIds.Count() > 0)
            {
                return await _rolePermissionRepository.GetAll().AnyAsync(aa => userRoleIds.Contains(aa.RoleId) && aa.Name == permissionName);
            }
            else
            {
                return await _userPermissionRepository.GetAll().AnyAsync(aa => aa.UserId == userId && aa.Name == permissionName);
            }
        }


    }
}
