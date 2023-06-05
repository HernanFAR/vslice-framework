using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VSlices.Core.Abstracts.Presentation;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static void UseEndpointDefinitions<T>(this T app)
        where T : IHost, IEndpointRouteBuilder
    {
        using var services = app.Services.CreateScope();

        var endpoints = services.ServiceProvider.GetServices<ISimpleEndpointDefinition>();

        foreach (var endpoint in endpoints)
        {
            endpoint.DefineEndpoint(app);
        }
    }
}
