using Drypoint.Application.Authorization.Users.Dto;
using Drypoint.Application.Services;
using Drypoint.Unity.BaseDto;
using Drypoint.Unity.BaseDto.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Drypoint.Application.Authorization.Users
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserAppService : IApplicationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<PagedResultDto<UserListDto>> GetUsersAsync(GetUsersInput input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GetUserForEditOutput> GetUserForEditAsync(NullableIdDto<long> input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task CreateOrUpdateUserAsync(CreateOrUpdateUserInput input);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DeleteUser(EntityDto<long> input);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task UnlockUser(EntityDto<long> input);
    }
}
