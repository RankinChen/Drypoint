using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseDto.Interface
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public interface IEntityDto<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
    }
}
