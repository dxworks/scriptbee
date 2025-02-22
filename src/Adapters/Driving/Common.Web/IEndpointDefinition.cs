using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ScriptBee.Common.Web;

public interface IEndpointDefinition
{
    void DefineServices(IServiceCollection services)
    {
    }

    void DefineEndpoints(IEndpointRouteBuilder app);
}
