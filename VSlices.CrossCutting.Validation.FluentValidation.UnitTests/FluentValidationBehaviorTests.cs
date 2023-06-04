using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.Validation.FluentValidation.UnitTests;

public class FluentValidationBehaviorTests
{
    public record Request : IRequest<Success>;

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess()
    {
        var request = new Request();
        var response = new Success();

        var validator = Mock.Of<IValidator<Request>>();
        var validatorMock = Mock.Get(validator);

        validatorMock.Setup(e => e.ValidateAsync(request, default))
            .Returns(Task.FromResult(new ValidationResult()));

        var validationBehaviorMock = new Mock<FluentValidationBehavior<Request, Success>>(new List<IValidator<Request>> { validator });
        var validationBehavior = validationBehaviorMock.Object;

        var handlerResponse = await validationBehavior.HandleAsync(request, async () => response);

        handlerResponse.IsT0.Should().BeTrue();
        handlerResponse.AsT0.Should().Be(response);

    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure()
    {
        var request = new Request();
        var expMessage = "Error de ejemplo";;

        var validator = Mock.Of<IValidator<Request>>();
        var validatorMock = Mock.Get(validator);

        validatorMock.Setup(e => e.ValidateAsync(request, default))
            .Returns(Task.FromResult(new ValidationResult(new []{ new ValidationFailure("", expMessage) })));

        var validationBehaviorMock = new Mock<FluentValidationBehavior<Request, Success>>(new List<IValidator<Request>> { validator }) { CallBase = true};
        var validationBehavior = validationBehaviorMock.Object;

        var handlerResponse = await validationBehavior.HandleAsync(request, async () => throw new Exception());

        handlerResponse.IsT1.Should().BeTrue();
        handlerResponse.AsT1.Kind.Should().Be(FailureKind.Validation);
        handlerResponse.AsT1.Errors.Should().ContainSingle(e => e == expMessage);

    }
}