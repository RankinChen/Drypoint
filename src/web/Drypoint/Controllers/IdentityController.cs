using System.Linq;
using Drypoint.Unity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drypoint.Controllers
{
    [ApiExplorerSettings(GroupName = DrypointConsts.AdminAPIGroupName)]
    [Route("identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(
                from c in User.Claims select new { c.Type, c.Value });
        }
    }
}