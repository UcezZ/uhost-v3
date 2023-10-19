using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uhost.Core.Extensions;
using Uhost.Web.Common;

namespace Uhost.Web.Middleware
{
    public class AuthorizationResultTransformer : IAuthorizationMiddlewareResultHandler
    {
        private readonly IAuthorizationMiddlewareResultHandler _handler;

        public AuthorizationResultTransformer()
        {
            _handler = new AuthorizationMiddlewareResultHandler();
        }

        public async Task HandleAsync(RequestDelegate requestDelegate, HttpContext httpContext, AuthorizationPolicy authorizationPolicy, PolicyAuthorizationResult policyAuthorizationResult)
        {
            if (policyAuthorizationResult.Forbidden && policyAuthorizationResult.AuthorizationFailure != null)
            {
                var req = policyAuthorizationResult
                    .AuthorizationFailure
                    .FailedRequirements
                    .OfType<RightRequirement>()
                    .FirstOrDefault();

                if (req != null)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    httpContext.Response.ContentType = ResponseHelper.СontentTypeJson;
                    await httpContext.Response.WriteAsync(ResponseHelper.MakeErrorData(req.Message).ToJson());

                    return;
                }
            }
            // Other transformations here

            await _handler.HandleAsync(requestDelegate, httpContext, authorizationPolicy, policyAuthorizationResult);
        }
    }
}
