using FluentAssertions;
using Moq;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.Validation.UnitTests;

public class AbstractValidationBehaviorTests
{
    public record Request : IRequest<Success>;

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess()
    {
        var validationBehavior = Mock.Of<AbstractValidationBehavior<Request, Success>>();
        var validationBehaviorMock = Mock.Get(validationBehavior);

        var request = new Request();
        var response = new Success();

        validationBehaviorMock.Setup(e => e.ValidateAsync(request, default))
            .Returns(ValueTask.FromResult<OneOf<Success, BusinessFailure>>(response));

        var handlerResponse = await validationBehavior.HandleAsync(request, async () => response);

        handlerResponse.IsT0.Should().BeTrue();
        handlerResponse.AsT0.Should().Be(response);

    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure()
    {
        var validationBehavior = Mock.Of<AbstractValidationBehavior<Request, Success>>();
        var validationBehaviorMock = Mock.Get(validationBehavior);

        var request = new Request();
        var response = BusinessFailure.Of.Validation("Error de ejemplo");

        validationBehaviorMock.Setup(e => e.ValidateAsync(request, default))
            .Returns(ValueTask.FromResult<OneOf<Success, BusinessFailure>>(response));

        var handlerResponse = await validationBehavior.HandleAsync(request, async () => throw new Exception());

        handlerResponse.IsT1.Should().BeTrue();
        handlerResponse.AsT1.Should().Be(response);

    }
}