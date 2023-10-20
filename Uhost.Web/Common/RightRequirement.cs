using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;
using Uhost.Web.Properties;
using static Uhost.Core.Data.Entities.Right;

namespace Uhost.Web.Common
{
    public class RightRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Правило сочетания коллекции
        /// </summary>
        public enum CombinationRule
        {
            /// <summary>
            /// Сочетание по И
            /// </summary>
            And,

            /// <summary>
            /// Сочетание по ИЛИ
            /// </summary>
            Or
        }

        public IEnumerable<Rights> Rights { get; }
        public CombinationRule Rule { get; }
        public string Message => Rule switch
        {
            CombinationRule.And => ApiStrings.Right_Error_ShouldHaveAll.Format(Rights.Select(e => e.TranslateEnumValue()).Join("; ")),
            CombinationRule.Or => ApiStrings.Right_Error_ShouldHaveAny.Format(Rights.Select(e => e.TranslateEnumValue()).Join("; ")),
            _ => throw new InvalidOperationException()
        };

        public RightRequirement(IEnumerable<Rights> rights, CombinationRule rule)
        {
            Rights = rights;
            Rule = rule;
        }
    }
}
