using Drypoint.Unity.BaseDto;
using System;
using System.Collections.Generic;

namespace Drypoint.Application.Authorization.Users.Dto
{
    /// <summary>
    /// 用户列表
    /// </summary>
    public class UserListDto : EntityDto<long>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? ProfilePictureId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEmailConfirmed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<UserListRoleDto> Roles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}