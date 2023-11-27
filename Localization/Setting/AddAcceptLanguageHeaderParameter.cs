using Localization.Domain.Enums;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;

public class AddAcceptLanguageHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
        {
            operation.Parameters = new List<OpenApiParameter>();
        }

        // Add "Accept-Language" header to the Swagger documentation
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Accept-Language",
            In = ParameterLocation.Header,
            Description = "Language preference for the request",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Enum = Enum.GetNames(typeof(Language)).Select(name => (IOpenApiAny)new OpenApiString(name)).ToList(),
                Default = new OpenApiString(Language.en.ToString())
            }
        });
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "tenant",
            In = ParameterLocation.Header,
            Description = "tenant parameter for the request",
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string"
                // You can customize the schema as needed for the talent parameter
            }
        });
    }
}
