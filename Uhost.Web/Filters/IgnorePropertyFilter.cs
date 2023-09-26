using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using Uhost.Core.Attributes;

namespace Uhost.Web.Filters
{
    public class IgnorePropertyFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription == null || operation.Parameters == null)
            {
                return;
            }

            if (!context.ApiDescription.ParameterDescriptions.Any())
            {
                return;
            }

            foreach (BindingSource bs in new BindingSource[] { BindingSource.Form, BindingSource.Query, BindingSource.Body })
            {
                var excludedProperties = context.ApiDescription.ParameterDescriptions.Where(p =>
                    p.Source.Equals(bs));
                if (!excludedProperties.Any())
                {
                    continue;
                }

                foreach (var excludedPropertie in excludedProperties)
                {
                    foreach (var customAttribute in excludedPropertie.CustomAttributes())
                    {
                        if (customAttribute.GetType() == typeof(SwaggerIgnoreAttribute))
                        {
                            // Удалим GET параметр
                            var parameter = operation.Parameters.FirstOrDefault(
                                parameter => string.Equals(
                                    parameter.Name, excludedPropertie.Name, StringComparison.Ordinal)
                            );
                            if (parameter != null)
                            {
                                operation.Parameters.Remove(parameter);
                            }

                            // Удалим POST параметры
                            if (operation.RequestBody == null)
                            {
                                continue;
                            }
                            for (int i = 0; i < operation.RequestBody.Content.Values.Count; i++)
                            {
                                for (int j = 0; j < operation.RequestBody.Content.Values.ElementAt(i).Encoding.Count; j++)
                                {
                                    if (operation.RequestBody.Content.Values.ElementAt(i).Encoding.ElementAt(j).Key == excludedPropertie.Name)
                                    {
                                        operation.RequestBody.Content.Values.ElementAt(i).Encoding
                                            .Remove(operation.RequestBody.Content.Values.ElementAt(i).Encoding.ElementAt(j));
                                        operation.RequestBody.Content.Values.ElementAt(i).Schema.Properties.Remove(excludedPropertie.Name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
