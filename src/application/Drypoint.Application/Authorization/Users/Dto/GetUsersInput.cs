using Drypoint.Unity.BaseDto.Input;
using Drypoint.Unity.BaseDto.Interface;

namespace Drypoint.Application.Authorization.Users.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class GetUsersInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// 
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Permission { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? RoleId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool OnlyLockedUsers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Name";
            }
        }
    }
}