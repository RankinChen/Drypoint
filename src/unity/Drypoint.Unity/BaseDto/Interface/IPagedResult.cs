using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseDto.Interface
{
    /// <summary>
    /// 返回结果 包含分页相关 接口
    /// </summary>
    public interface IPagedResult<T> : IListResult<T>, IHasTotalCount
    {

    }
}
