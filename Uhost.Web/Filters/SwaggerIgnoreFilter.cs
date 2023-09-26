using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Reflection;
using Uhost.Core.Attributes;

namespace Uhost.Web.Filters
{
    public class SwaggerIgnoreFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext schemaFilterContext)
        {
            if (schema?.Properties == null)
            {
                return;
            }

            var excludedProperties = schemaFilterContext.Type
                .GetProperties()
                .Where(t => t.GetCustomAttribute<SwaggerIgnoreAttribute>() != null);

            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.ContainsKey(excludedProperty.Name))
                {
                    schema.Properties.Remove(excludedProperty.Name);
                }
            }
        }
    }
}
