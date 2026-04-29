using System.ComponentModel;
using Microsoft.AspNetCore.OpenApi;

namespace ScriptBee.Common.Web.Extensions;

public static class OpenApiExtensions
{
    extension(OpenApiOptions options)
    {
        public OpenApiOptions StripWebPrefix()
        {
            options.CreateSchemaReferenceId = (type) =>
            {
                var defaultId = OpenApiOptions.CreateDefaultSchemaReferenceId(type);

                if (defaultId is not null && defaultId.StartsWith("Web"))
                {
                    return defaultId[3..];
                }

                return defaultId;
            };

            return options;
        }

        public OpenApiOptions AddDescriptionSupport()
        {
            options.AddSchemaTransformer(
                (schema, context, _) =>
                {
                    var descriptionAttribute = context
                        .JsonTypeInfo.Type.GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .Cast<DescriptionAttribute>()
                        .FirstOrDefault();

                    if (descriptionAttribute != null)
                    {
                        schema.Description = descriptionAttribute.Description;
                    }

                    return Task.CompletedTask;
                }
            );

            return options;
        }
    }
}
