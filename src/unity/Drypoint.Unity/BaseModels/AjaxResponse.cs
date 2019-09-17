using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Unity.BaseModels
{
    [Serializable]
    public class AjaxResponse : AjaxResponse<object>
    {
        public AjaxResponse()
        {

        }
        
        public AjaxResponse(bool success)
            : base(success)
        {

        }

        public AjaxResponse(object result)
            : base(result)
        {

        }

        public AjaxResponse(ErrorInfo error, bool unAuthorizedRequest = false)
            : base(error, unAuthorizedRequest)
        {

        }
    }
}
