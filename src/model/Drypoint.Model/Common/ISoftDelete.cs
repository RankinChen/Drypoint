using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Model.Common
{
    /// <summary>
    /// 包含 逻辑删除属性
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        /// 逻辑删除
        /// </summary>
        bool IsDeleted { get; set; }
    }
}
