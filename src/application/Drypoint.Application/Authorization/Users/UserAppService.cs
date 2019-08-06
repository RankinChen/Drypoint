﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Drypoint.Application.Authorization.Users.Dto;
using Drypoint.Application.Services;
using Drypoint.Unity.BaseDto.Output;
using Drypoint.Core.Authorization.Roles;
using Drypoint.Core.Authorization.Users;
using Drypoint.EntityFrameworkCore.Repositories;
using Drypoint.Unity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using Drypoint.Unity.BaseDto;
using Drypoint.Unity.Extensions;
using Drypoint.Unity.Extensions.Collections;

namespace Drypoint.Application.Authorization.Users
{
    //[ApiExplorerSettings(GroupName = DrypointConsts.AdminAPIGroupName)]
    [Route(DrypointConsts.ApiPrefix + "User")]
    public class UserAppService : ApplicationService, IUserAppService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role, int> _roleRepository;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionRepository;
        private readonly IRepository<UserPermissionSetting, long> _userPermissionRepository;

        public UserAppService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserAppService> logger,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            IRepository<User, long> userRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role, int> roleRepository,
            IRepository<RolePermissionSetting, long> rolePermissionRepository,
            IRepository<UserPermissionSetting, long> userPermissionRepository
            )



        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _userPermissionRepository = userPermissionRepository;
        }

        public async Task CreateOrUpdateUserAsync(CreateOrUpdateUserInput input)
        {
            if (input.User.Id.HasValue)
            {
                await UpdateUserAsync(input);
            }
            else
            {
                await CreateUserAsync(input);
            }
        }

        private async Task UpdateUserAsync(CreateOrUpdateUserInput input)
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(aa => aa.Id == Convert.ToInt64(input.User.Id.Value));

            _mapper.Map(input.User, user);

            if (input.SetRandomPassword)
            {
                user.Password = _passwordHasher.HashPassword(user, CreateRandomPassword());
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                user.Password = input.User.Password;
            }


            #region  Update roles
            await _userRoleRepository.DeleteAsync(aa => aa.UserId == user.Id);

            var addRoleId = _roleRepository.GetAll().Where(aa => input.AssignedRoleNames.Contains(aa.Name)).Select(aa => aa.Id).ToList();
            foreach (var roleId in addRoleId)
            {
                await _userRoleRepository.InsertAsync(
                    new UserRole
                    {
                        RoleId = roleId,
                        UserId = user.Id
                    });
            }
            #endregion

            if (input.SendActivationEmail)
            {
                //TODO

            }

            await _userRepository.UpdateAsync(user);
        }

        private async Task CreateUserAsync(CreateOrUpdateUserInput input)
        {
            var user = _mapper.Map<User>(input.User); //Passwords is not mapped (see mapping configuration)

            //Set password
            if (input.SetRandomPassword)
            {
                user.Password = _passwordHasher.HashPassword(user, CreateRandomPassword());
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                user.Password = _passwordHasher.HashPassword(user, input.User.Password);
            }

            //user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            //Assign roles
            var addRoleId = _roleRepository.GetAll().Where(aa => input.AssignedRoleNames.Contains(aa.Name)).Select(aa => aa.Id).ToList();
            foreach (var roleId in addRoleId)
            {
                await _userRoleRepository.InsertAsync(
                    new UserRole
                    {
                        RoleId = roleId,
                        UserId = user.Id
                    });
            }
            await _userRepository.InsertAsync(user);


            if (input.SendActivationEmail)
            {
                //Send Email TODO 
            }
        }


        public async Task DeleteUser(EntityDto<long> input)
        {
            await _userRepository.DeleteAsync(aa => aa.Id == input.Id);
        }

        public async Task<GetUserForEditOutput> GetUserForEditAsync(NullableIdDto<long> input)
        {
            var userRoleDtos = await _roleRepository.GetAll()
                                .OrderBy(r => r.DisplayName)
                                .Select(r => new UserRoleDto
                                {
                                    RoleId = r.Id,
                                    RoleName = r.Name,
                                    RoleDisplayName = r.DisplayName
                                }).ToArrayAsync();


            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos,
                MemberedOrganizationUnits = new List<string>()
            };

            if (!input.Id.HasValue)
            {
                //Creating a new user
                output.User = new UserEditDto
                {
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    //TODO
                    IsTwoFactorEnabled = false,
                    IsLockoutEnabled = false
                };

                foreach (var defaultRole in await _roleRepository.GetAll().Where(r => r.IsDefault).ToListAsync())
                {
                    var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                    if (defaultUserRole != null)
                    {
                        defaultUserRole.IsAssigned = true;
                    }
                }
            }
            else
            {
                //Editing an existing user
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(aa => aa.Id == input.Id.Value);

                output.User = _mapper.Map<UserEditDto>(user);

                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await _userRepository.GetAll().Include(aa => aa.Roles).AnyAsync(aa => aa.Roles.Any(bb => bb.RoleId == userRoleDto.RoleId));
                }
            }
            return output;
        }

        public async Task<PagedResultDto<UserListDto>> GetUsersAsync(GetUsersInput input)
        {
            var query = _userRepository.GetAll().WhereIf(input.RoleId.HasValue, u => u.Roles.Any(r => r.RoleId == input.RoleId.Value))
                .WhereIf(input.OnlyLockedUsers, u => u.LockoutEndDateUtc.HasValue && u.LockoutEndDateUtc.Value > DateTime.UtcNow)
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter)
                );

            if (!input.Permission.IsNullOrWhiteSpace())
            {
                query = from user in query
                        join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId into urJoined
                        from ur in urJoined.DefaultIfEmpty()
                        join up in _userPermissionRepository.GetAll() on new { UserId = user.Id, Name = input.Permission } equals new { up.UserId, up.Name } into upJoined
                        from up in upJoined.DefaultIfEmpty()
                        join rp in _rolePermissionRepository.GetAll() on new { RoleId = ur == null ? 0 : ur.RoleId, Name = input.Permission } equals new { rp.RoleId, rp.Name } into rpJoined
                        from rp in rpJoined.DefaultIfEmpty()
                        where up != null && up.IsGranted || up == null && rp != null
                        group user by user
                        into userGrouped
                        select userGrouped.Key;
            }

            var userCount = await query.CountAsync();

            var users = query.OrderBy(input.Sorting)
                .PageBy(input)
                .ToList();

            var userListDtos = _mapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return new PagedResultDto<UserListDto>(
                userCount,
                userListDtos
                );
        }

        public Task UnlockUser(EntityDto<long> input)
        {
            throw new NotImplementedException();
        }

        private async Task FillRoleNames(List<UserListDto> userListDtos)
        {
            var userRoles = await _userRoleRepository.GetAll().Include(aa => aa.Role)
                .Where(userRole => userListDtos.Any(aa => aa.Id == userRole.UserId))
                .Select(userRole => userRole).ToListAsync();

            var distinctRoleIds = userRoles.Select(userRole => userRole.RoleId).Distinct();

            foreach (var user in userListDtos)
            {
                var rolesOfUser = userRoles.Where(userRole => userRole.UserId == user.Id).ToList();
                user.Roles = _mapper.Map<List<UserListRoleDto>>(rolesOfUser);
            }

            var roleNames = new Dictionary<int, string>();
            foreach (var roleId in distinctRoleIds)
            {
                roleNames[roleId] = userRoles.FirstOrDefault(aa => aa.RoleId == roleId)?.Role.DisplayName;
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                }

                userListDto.Roles = userListDto.Roles.OrderBy(r => r.RoleName).ToList();
            }
        }
    }
}
