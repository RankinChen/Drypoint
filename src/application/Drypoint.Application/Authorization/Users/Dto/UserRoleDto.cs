namespace Drypoint.Application.Authorization.Users.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class UserRoleDto
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 角色显示名称
        /// </summary>
        public string RoleDisplayName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAssigned { get; set; }
    }
}