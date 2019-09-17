using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseDto.Interface
{
    /// <summary>
    /// 返回结果 包含总数量 接口
    /// </summary>
    public interface IHasTotalCount
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        int TotalCount { get; set; }
    }
}
