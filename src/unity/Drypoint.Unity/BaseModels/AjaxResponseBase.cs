using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseModels
{
    public abstract class AjaxResponseBase
    {
        public string TargetUrl { get; set; }


        public bool Success { get; set; }

        public ErrorInfo Error { get; set; }

        public bool UnAuthorizedRequest { get; set; }

    }
}
