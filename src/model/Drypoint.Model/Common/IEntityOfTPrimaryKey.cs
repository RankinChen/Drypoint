using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Model.Common
{
    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }
        
        bool IsTransient();
    }
}
