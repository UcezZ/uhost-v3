using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Uhost.Core.Extensions;
using Uhost.Core.Services.User;
using Uhost.Web.Common;
using static Uhost.Web.Common.RightRequirement;

namespace Uhost.Web.Middleware
{
    public class RightAuthHandler : AuthorizationHandler<RightRequirement>
    {
        private readonly IUserService _userService;

        public RightAuthHandler(IUserService userService)
        {
            _userService = userService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RightRequirement requirement)
        {
            if (context.User.TryGetUserId(out var userId))
            {
                var user = _userService.GetAccessData(userId);

                switch (requirement.Rule)
                {
                    case CombinationRule.Or:
                        if (user.Rights.ContainsAny(requirement.Rights))
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                        }
                        break;
                    case CombinationRule.And:
                        if (user.Rights.ContainsAll(requirement.Rights))
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                        }
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
