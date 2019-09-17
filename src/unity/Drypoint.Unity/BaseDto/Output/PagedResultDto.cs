using Drypoint.Unity.BaseDto.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseDto.Output
{
    /// <summary>
    /// 返回分页列表结果集
    /// </summary>
    public class PagedResultDto<T>:ListResultDto<T>, IPagedResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public PagedResultDto()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="items"></param>
        public PagedResultDto(int totalCount, IReadOnlyList<T> items)
            : base(items)
        {
            TotalCount = totalCount;
        }
    }
}
