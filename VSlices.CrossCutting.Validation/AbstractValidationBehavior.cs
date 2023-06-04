using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.Validation;

public abstract class AbstractValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);

        if (validationResult.IsT1)
        {
            return validationResult.AsT1;
        }

        return await next();
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateAsync(TRequest request, CancellationToken cancellationToken = default);
}
