using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Sender;

namespace VSlices.Core.Sender.Reflection;

public abstract class AbstractHandlerWrapper
{
    public abstract ValueTask<OneOf<object?, BusinessFailure>> HandleAsync(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default);
}

public abstract class AbstractHandlerWrapper<TResponse> : AbstractHandlerWrapper
{
    public abstract ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(IRequest<TResponse> request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

public class RequestHandlerWrapper<TRequest, TResponse> : AbstractHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    public override async ValueTask<OneOf<object?, BusinessFailure>> HandleAsync(
        object request, IServiceProvider serviceProvider, CancellationToken cancellationToken = default) =>
        await HandleAsync((IRequest<TResponse>)request, serviceProvider, cancellationToken);

    public override ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(
        IRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        ValueTask<OneOf<TResponse, BusinessFailure>> Handler()
        {
            return serviceProvider.GetRequiredService<IHandler<TRequest, TResponse>>()
                .HandleAsync((TRequest)request, cancellationToken);
        }

        return serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<TResponse>)Handler,
                (next, pipeline) => () => pipeline.HandleAsync((TRequest)request, next, cancellationToken))();
    }
}

public class ReflectionSender : ISender
{
    private static readonly ConcurrentDictionary<Type, AbstractHandlerWrapper> RequestHandlers = new();

    private readonly IServiceProvider _serviceProvider;

    public ReflectionSender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ValueTask<OneOf<TResponse, BusinessFailure>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handler = (AbstractHandlerWrapper<TResponse>)RequestHandlers.GetOrAdd(request.GetType(), static requestType =>
        {
            var wrapperType = typeof(RequestHandlerWrapper<,>).MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
            return (AbstractHandlerWrapper)wrapper;
        });

        return handler.HandleAsync(request, _serviceProvider, cancellationToken);
    }
}
