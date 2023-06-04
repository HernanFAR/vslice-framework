using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.Validation.FluentValidation;

public class FluentValidationBehavior<TRequest, TResponse> : AbstractValidationBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    private readonly IValidator<TRequest>? _requestValidator;

    public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> requestValidators)
    {
        _requestValidator = requestValidators.FirstOrDefault();
    }

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        if (_requestValidator is null)
        {
            return new Success();
        }

        var requestValidationResult = await _requestValidator.ValidateAsync(request, cancellationToken);

        if (requestValidationResult.IsValid) return new Success();

        var errors = requestValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);
    }
}
