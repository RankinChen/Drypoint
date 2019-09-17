using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseDto.Interface
{
    /// <summary>
    /// 返回结果 包含数据集合 接口
    /// </summary>
    public interface IListResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        IReadOnlyList<T> Items { get; set; }
    }
}
