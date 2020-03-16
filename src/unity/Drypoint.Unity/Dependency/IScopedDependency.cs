using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.Dependency
{
    /// <summary>
    /// 实现此接口将自动注册DI 
    /// 详情见ServiceCollectionDIExtensions类 或AutofacDIExtensionsModule
    /// 暂时生存期服务是每次从服务容器进行请求时创建的。 这种生存期适合轻量级、 无状态的服务。
    /// 参考
    /// https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.2
    /// </summary>
    public interface IScopedDependency
    {
    }
}
