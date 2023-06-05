using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using OneOf.Types;
using OneOf;
using VSlices.Core.Abstracts.Responses;
using NotFound = Microsoft.AspNetCore.Http.HttpResults.NotFound;

namespace VSlices.Core.Abstracts.UnitTests.Extensions;

public class OneOfExtensionsTests
{
    [Fact]
    public void MatchEndpointResult_ShouldCallSuccessFunction()
    {
        OneOf<Success, BusinessFailure> oneOf = new Success();

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<Ok>();
    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnForbidHttpResult()
    {
        OneOf<Success, BusinessFailure> oneOf = BusinessFailure.Of.NotAllowedUser();

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<ForbidHttpResult>();
    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnNotFound()
    {
        OneOf<Success, BusinessFailure> oneOf = BusinessFailure.Of.NotFoundResource();

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnNotFound_DetailWithErrors()
    {
        OneOf<Success, BusinessFailure> oneOf = BusinessFailure.Of.NotFoundResource(new []{ "XD" });

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<NotFound<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnConflict()
    {
        OneOf<Success, BusinessFailure> oneOf = BusinessFailure.Of.ConcurrencyError();

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<Conflict>();
    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnConflict_DetailWithErrors()
    {
        OneOf<Success, BusinessFailure> oneOf = BusinessFailure.Of.ConcurrencyError(new []{ "XD" });

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<Conflict<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithErrors()
    {
        OneOf<Success, BusinessFailure> oneOf = BusinessFailure.Of.ContractValidation(new []{ "XD" });

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<UnprocessableEntity<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithError()
    {
        OneOf<Success, BusinessFailure> oneOf = BusinessFailure.Of.ContractValidation("XD");

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<UnprocessableEntity<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithErrorsAndDomainValidation()
    {
        OneOf<Success, BusinessFailure> oneOf = BusinessFailure.Of.DomainValidation(new []{ "XD" });

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<UnprocessableEntity<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithErrorAndDomainValidation()
    {
        OneOf<Success, BusinessFailure> oneOf = BusinessFailure.Of.DomainValidation("XD");

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<UnprocessableEntity<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldThrowArgumentOutOfRange()
    {
        OneOf<Success, BusinessFailure> oneOf = new BusinessFailure((FailureKind)10, Array.Empty<string>());

        var act = () => oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        act.Should().ThrowExactly<ArgumentOutOfRangeException>();

    }
}