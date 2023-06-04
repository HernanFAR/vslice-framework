using OneOf;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.BusinessLogic;

public interface IHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

public delegate ValueTask<OneOf<TResponse, BusinessFailure>> RequestHandlerDelegate<TResponse>();

public interface IPipelineBehavior<in TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
{
    ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default);
}