using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Drypoint.Application.Authorization;
using Drypoint.Application.Custom.Demo;
using Drypoint.EntityFrameworkCore.EntityFrameworkCore;
using Drypoint.Unity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drypoint.Controllers
{
    [ApiExplorerSettings(GroupName = DrypointConsts.AdminAPIGroupName)]
    [Route("api/[controller]")]
    public class Values1Controller : ControllerBase
    {
        public readonly IDemoAppService _demoAppService;

        public Values1Controller(
            IDemoAppService demoAppService
            )
        {
            _demoAppService = demoAppService;
        }

        // GET api/values
        [HttpGet]
        [DrypointAuthorize("aaaaa","BBBB")]
        public ActionResult<IEnumerable<string>> Get()
        {
            var result = _demoAppService.GetAll();

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
