using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseDto
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public class NullableIdDto<TId> where TId : struct
    {
        /// <summary>
        /// 
        /// </summary>
        public NullableIdDto() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public NullableIdDto(TId? id) { }
        /// <summary>
        /// 
        /// </summary>
        public TId? Id { get; set; }
    }
}
