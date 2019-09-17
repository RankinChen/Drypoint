
using Drypoint.Unity.BaseDto.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Drypoint.Unity.BaseDto.Input
{ 
    /// <summary>
    /// 
    /// </summary>
public class PagedInputDto : IPagedBaseRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [Range(1, DrypointConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PagedInputDto()
        {
            MaxResultCount = DrypointConsts.DefaultPageSize;
        }
    }
}
