using Microsoft.AspNetCore.Http;
using VSlices.Core.Abstracts.Responses;

// ReSharper disable once CheckNamespace
namespace OneOf;

public static class OneOfExtensions
{
    public static IResult MatchEndpointResult<TSuccess>(this OneOf<TSuccess, BusinessFailure> result,
        Func<TSuccess, IResult> successFunc)
    {
        return result.Match(
            successFunc,
            e =>
            {
                return e.Kind switch
                {
                    FailureKind.UserNotAllowed => TypedResults.Forbid(),
                    FailureKind.NotFoundResource => e.Errors.Any() ? TypedResults.NotFound(e.Errors) : TypedResults.NotFound(),
                    FailureKind.ConcurrencyError => e.Errors.Any() ? TypedResults.Conflict(e.Errors) : TypedResults.Conflict(),
                    FailureKind.ContractValidation => TypedResults.UnprocessableEntity(e.Errors),
                    FailureKind.DomainValidation => TypedResults.UnprocessableEntity(e.Errors),
                    _ => throw new ArgumentOutOfRangeException(nameof(e.Kind), "A not valid FailureKind value was returned")
                };
            });
    }
}
