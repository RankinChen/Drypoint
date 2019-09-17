using Drypoint.Unity.BaseDto.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseDto.Output
{
    /// <summary>
    /// 返回列表结果集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListResultDto<T> : IListResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<T> Items
        {
            get { return _items ?? (_items = new List<T>()); }
            set { _items = value; }
        }
        private IReadOnlyList<T> _items;

        /// <summary>
        /// 
        /// </summary>
        public ListResultDto()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public ListResultDto(IReadOnlyList<T> items)
        {
            Items = items;
        }
    }
}
