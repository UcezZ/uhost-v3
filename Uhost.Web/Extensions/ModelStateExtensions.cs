using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using Uhost.Web.Properties;

namespace Uhost.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static object GetErrors(this ModelStateDictionary modelState)
        {
            var erroneousFields = modelState
                .Where(ms => ms.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });

            var oe = new Dictionary<string, string[]>();

            foreach (var erroneousField in erroneousFields)
            {
                var fieldKey = erroneousField.Key;
                var fieldErrors = erroneousField.Errors.Select(error => error.ErrorMessage).ToArray();

                if (fieldKey.StartsWith("$"))
                {
                    fieldKey = fieldKey?.Trim().Trim('.', '$') ?? string.Empty;
                    fieldErrors = new[] { ApiStrings.Common_Error_Invalid };
                }

                oe[fieldKey] = fieldErrors;
            }

            return oe;
        }
    }
}
