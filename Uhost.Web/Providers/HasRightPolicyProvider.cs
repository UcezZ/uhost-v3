using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uhost.Core.Extensions;
using Uhost.Web.Attributes.Authorize;
using Uhost.Web.Common;
using static System.Console;
using static Uhost.Core.Data.Entities.Right;
using static Uhost.Web.Common.RightRequirement;

namespace Uhost.Core.Providers
{
    public class HasRightPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public HasRightPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        private static bool ResolveRights(string policyName, out CombinationRule rule, out ICollection<Rights> rights)
        {
            if (policyName.StartsWith(HasRightAuthorizeAttribute.PolicyPrefixOr))
            {
                rule = CombinationRule.Or;
                policyName = policyName[HasRightAuthorizeAttribute.PolicyPrefixOr.Length..];
            }
            else if (policyName.StartsWith(HasRightAuthorizeAttribute.PolicyPrefixAnd))
            {
                rule = CombinationRule.And;
                policyName = policyName[HasRightAuthorizeAttribute.PolicyPrefixAnd.Length..];
            }
            else
            {
                throw new InvalidOperationException();
            }

            if (policyName.TryParseMany<int>(HasRightAuthorizeAttribute.Delimiter, out var rightIds))
            {
                rights = rightIds
                    .Distinct()
                    .Select(e => (Rights)e)
                    .ToList();

                return true;
            }

            rights = default;

            return false;
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            WriteLine(policyName);
            bool useOr = policyName.StartsWith(HasRightAuthorizeAttribute.PolicyPrefixOr);
            bool useAnd = policyName.StartsWith(HasRightAuthorizeAttribute.PolicyPrefixAnd);

            if (useOr != useAnd)
            {
                if (ResolveRights(policyName, out var rule, out var rights))
                {
                    WriteLine(string.Join(", ", rights.Select(e => $"{(int)e}: {e}")));

                    var policy = new AuthorizationPolicyBuilder();
                    policy.AddRequirements(new RightRequirement(rights.Select(e => e), rule));

                    return Task.FromResult(policy.Build());
                }
            }

            return Task.FromResult<AuthorizationPolicy>(null);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
    }
}
