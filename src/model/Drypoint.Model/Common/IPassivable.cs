using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Model.Common
{
    /// <summary>
    /// 含是否激活属性
    /// </summary>
    public interface IPassivable
    {
        /// <summary>
        /// 是否激活
        /// </summary>
        bool IsActive { get; set; }
    }
}
