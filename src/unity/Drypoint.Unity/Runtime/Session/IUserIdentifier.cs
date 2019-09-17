using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.Runtime.Session
{
    public interface IUserIdentifier
    {
        long UserId { get; }
        long? RoleId { get; }
    }
}
