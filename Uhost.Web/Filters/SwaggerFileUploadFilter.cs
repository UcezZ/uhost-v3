using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using Uhost.Web.Attributes;

namespace Uhost.Web.Filters
{
    /// <summary>
    /// Позволяет методу контроллера принимать файлы в Swagger
    /// </summary>
    public class SwaggerFileUploadFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var mime = "multipart/form-data";
            if (!context.MethodInfo.CustomAttributes.Where(e => e.AttributeType == typeof(FileUploadAttribute)).Any() ||
                operation.RequestBody == null ||
                !operation.RequestBody.Content.Any(e => e.Key.Equals(mime, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            var param = context.MethodInfo.GetParameters().Where(p => p.ParameterType == typeof(IFormFile));
            operation.RequestBody.Content[mime].Schema.Properties = param.ToDictionary(
                k => k.Name,
                v => new OpenApiSchema()
                {
                    Type = "string",
                    Format = "binary"
                });
        }
    }
}
