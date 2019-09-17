using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.Dependency
{
    /// <summary>
    /// 实现此接口将自动注册DI 
    /// 仅注入Drypoint.Host.Core
    ///       Drypoint.Application
    ///       Drypoint.Application.Custom
    ///       Drypoint.EntityFrameworkCore
    /// 详情见ServiceCollectionDIExtensions类
    /// 注册单例服务
    /// 单一实例生存期服务是在第一次请求时（或者在运行 ConfigureServices 并且使用服务注册指定实例时）创建的。 每个后续请求都使用相同的实例。
    /// 参考
    /// https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.2
    /// </summary>
    public interface ISingletonDependency
    {
    }
}
