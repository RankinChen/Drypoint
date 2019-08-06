﻿namespace Drypoint.Application.Authorization.Users.Dto
{
    /// <summary>
    /// 用户角色列表
    /// </summary>
    public class UserListRoleDto
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
    }
}