using Drypoint.Unity.BaseDto.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseDto.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class PagedAndSortedInputDto : PagedInputDto, ISortedBaseInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string Sorting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PagedAndSortedInputDto()
        {
            MaxResultCount = Drypoint.Unity.DrypointConsts.DefaultPageSize;
        }
    }
}
