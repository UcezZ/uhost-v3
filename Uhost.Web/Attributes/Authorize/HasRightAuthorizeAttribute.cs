using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;
using Uhost.Web.Common;
using static Uhost.Core.Data.Entities.Right;
using static Uhost.Web.Common.RightRequirement;

namespace Uhost.Web.Attributes.Authorize
{
    /// <summary>
    /// Валидация прав пользователя
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class HasRightAuthorizeAttribute : AuthorizeAttribute
    {
        public const string PolicyPrefixOr = "HasRightOr";
        public const string PolicyPrefixAnd = "HasRightAnd";
        public const char Delimiter = '|';
        private readonly IEnumerable<Rights> _rights;
        private readonly CombinationRule _rule;

        /// <summary>
        /// Коллекция прав
        /// </summary>
        public IEnumerable<Rights> Rights
        {
            get
            {
                return _rights;
            }
            private set
            {
                Policy = _rule switch
                {
                    CombinationRule.And => $"{PolicyPrefixAnd}{value.Select(e => $"{(int)e}").Join($"{Delimiter}")}",
                    CombinationRule.Or => $"{PolicyPrefixOr}{value.Select(e => $"{(int)e}").Join($"{Delimiter}")}",
                    _ => throw new ArgumentException(nameof(_rule)),
                };
            }
        }

        /// <summary>
        /// Правило сочетания
        /// </summary>
        public CombinationRule CombinationRule => _rule;

        /// <summary>
        /// Валидация прав пользователя
        /// </summary>
        /// <param name="rule">Правило сочетания прав</param>
        /// <param name="rights">Права</param>
        public HasRightAuthorizeAttribute(CombinationRule rule, params Rights[] rights)
        {
            _rule = rule;
            _rights = rights
                .Distinct()
                .OrderBy(e => e)
                .ToList();
        }

        /// <inheritdoc cref="HasRightAuthorizeAttribute"/>
        public HasRightAuthorizeAttribute(params Rights[] rights) : this(CombinationRule.And, rights) { }

        internal RightRequirement ToRequirement() => new RightRequirement(_rights, _rule);
    }
}
