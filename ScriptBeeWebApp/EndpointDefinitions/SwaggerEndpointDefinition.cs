using MicroElements.Swashbuckle.FluentValidation.AspNetCore;

namespace ScriptBeeWebApp.EndpointDefinitions;

public class SwaggerEndpointDefinition : IEndpointDefinition
{
    public void DefineServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddFluentValidationRulesToSwagger();
    }

    public void DefineEndpoints(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
