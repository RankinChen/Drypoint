
using Drypoint.Unity.BaseDto.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseDto
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class EntityDto<TPrimaryKey> : IEntityDto<TPrimaryKey>
    {
        /// <summary>
        /// 
        /// </summary>
        public EntityDto()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public EntityDto(TPrimaryKey id)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public TPrimaryKey Id { get; set; }
    }
}
