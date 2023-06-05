using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Interfaces;
using OneOf;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Sender;
using static VSlices.Core.Abstracts.UnitTests.Extensions.ServiceCollectionExtensionsTests;

namespace VSlices.Core.Abstracts.UnitTests.Extensions;

public class ServiceCollectionExtensionsTests
{
    public class Sender : ISender
    {
        public ValueTask<OneOf<TResponse, BusinessFailure>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class Behavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class Dependency { }

    public class Endpoint : IEndpointDefinition
    {
        public void DefineEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet("api/test", Test);
        }

        public static Task<IResult> Test(HttpContext context) => Task.FromResult<IResult>(EmptyHttpResult.Instance);

        public static void DefineDependencies(IServiceCollection services)
        {
            services.AddScoped<Dependency>();
        }
    }

    [Fact]
    public void AddSender_ShouldAddSender()
    {
        var services = new ServiceCollection();

        services.AddSender<Sender>();

        services
            .Where(e => e.ServiceType == typeof(ISender))
            .Where(e => e.ImplementationType == typeof(Sender))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

    }

    [Fact]
    public void AddEndpointDefinition_ShouldAddSimpleEndpointAndDependencies()
    {
        var services = new ServiceCollection();

        services.AddEndpointDefinition<Endpoint>();

        services
            .Where(e => e.ServiceType == typeof(Dependency))
            .Where(e => e.ImplementationType == typeof(Dependency))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();
        
        services
            .Where(e => e.ServiceType == typeof(ISimpleEndpointDefinition))
            .Where(e => e.ImplementationType == typeof(Endpoint))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

    }

    [Fact]
    public void AddPipelineBehavior_ShouldAddOpenBehavior()
    {
        var services = new ServiceCollection();

        services.AddPipelineBehavior(typeof(Behavior<,>));

        services
            .Where(e => e.ServiceType == typeof(IPipelineBehavior<,>))
            .Where(e => e.ImplementationType == typeof(Behavior<,>))
            .Any(e => e.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();

    }

    [Fact]
    public void AddPipelineBehavior_ShouldThrowInvalidOperation()
    {
        var services = new ServiceCollection();

        var act = () => services.AddPipelineBehavior(typeof(int));

        act.Should().Throw<InvalidOperationException>();
    }
}
