using System.Text.Json;
using System.Text.Json.Serialization;
using NJsonSchema;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Photobox.Web.Image;
using Photobox.Web.Photobox;

namespace Photobox.Web.OperationProcessors;

public class PhotoboxIdHeaderProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        bool hasHeader = context
            .MethodInfo.GetParameters()
            .Any(p =>
            {
                var fromHeaderAttr =
                    p.GetCustomAttributes(
                            typeof(Microsoft.AspNetCore.Mvc.FromHeaderAttribute),
                            inherit: true
                        )
                        .FirstOrDefault() as Microsoft.AspNetCore.Mvc.FromHeaderAttribute;

                // Match exactly "X-PhotoBox-Id"
                return fromHeaderAttr?.Name?.Equals(
                        "X-PhotoBox-Id",
                        StringComparison.OrdinalIgnoreCase
                    ) == true;
            });

        if (hasHeader)
        {
            context.OperationDescription.Operation.Parameters.Add(
                new OpenApiParameter
                {
                    Name = "X-PhotoBox-Id",
                    Kind = OpenApiParameterKind.Header,
                    Description = "Custom header to identify the PhotoBox",
                    IsRequired = false,
                    Schema = new JsonSchema { Type = JsonObjectType.String },
                }
            );
        }

        return true;
    }
}
