using Drypoint.Unity;
using Drypoint.Unity.BaseModels;
using Drypoint.Unity.Dependency;
using Drypoint.Unity.Extensions;
using DrypointException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drypoint.Core.Authorization
{
    public class AsyncAuthorizationFilter : IAsyncAuthorizationFilter, ITransientDependency
    {
        private readonly ILogger _logger;

        private readonly IAuthorizationHelper _authorizationHelper;

        public AsyncAuthorizationFilter(
            ILogger<AsyncAuthorizationFilter> logger,
            IAuthorizationHelper authorizationHelper)
        {
            _logger = logger;
            _authorizationHelper = authorizationHelper;
        }


        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                return;
            }

            if (!context.ActionDescriptor.IsControllerAction())
            {
                return;
            }

            try
            {
                await _authorizationHelper.AuthorizeAsync(
                    context.ActionDescriptor.GetMethodInfo(),
                    context.ActionDescriptor.GetMethodInfo().DeclaringType);
            }
            catch (AuthorizationException ex)
            {
                _logger.LogWarning(ex.ToString(), ex);

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    context.Result = new ObjectResult(new AjaxResponse(new ErrorInfo(ex.Message), true))
                    {
                        StatusCode = context.HttpContext.User.Identity.IsAuthenticated
                            ? (int)System.Net.HttpStatusCode.Forbidden
                            : (int)System.Net.HttpStatusCode.Unauthorized
                    };
                }
                else
                {
                    context.Result = new ChallengeResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), ex);

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    context.Result = new ObjectResult(new AjaxResponse(new ErrorInfo(ex.Message)))
                    {
                        StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
                    };
                }
                else
                {
                    context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}
