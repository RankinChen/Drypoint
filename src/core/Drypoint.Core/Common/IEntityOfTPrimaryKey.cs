using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Core.Common
{
    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
        
        bool IsTransient();
    }
}
