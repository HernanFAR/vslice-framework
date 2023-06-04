﻿using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Protected;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation.UnitTests.UpdateHandlers;


public class FullyFluentValidatedRemoveHandler_TwoGenerics
{
    public record Domain;
    public record Request : ICommand;
    public record Response;

    public class FullyFluentValidatedUpdateHandler : FullyFluentValidatedUpdateHandler<Request, Domain>
    {
        public FullyFluentValidatedUpdateHandler(IValidator<Request> requestValidator, IValidator<Domain> entityValidator, IUpdateRepository<Domain> repository) : base(requestValidator, entityValidator, repository) { }

        protected override ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(Request request, CancellationToken cancellationToken = default) 
            => ValueTask.FromResult<OneOf<Success, BusinessFailure>>(new Success());

        protected override ValueTask<Domain> GetAndProcessEntityAsync(Request request, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(new Domain());

    }

    private readonly Mock<IValidator<Request>> _mockedRequestValidator;
    private readonly Mock<IValidator<Domain>> _mockedDomainValidator;
    private readonly Mock<IUpdateRepository<Domain>> _mockedRepository;
    private readonly FullyFluentValidatedUpdateHandler _handler;

    public FullyFluentValidatedRemoveHandler_TwoGenerics()
    {
        _mockedRequestValidator = new Mock<IValidator<Request>>();
        _mockedDomainValidator = new Mock<IValidator<Domain>>();
        _mockedRepository = new Mock<IUpdateRepository<Domain>>();
        _handler = new FullyFluentValidatedUpdateHandler(_mockedRequestValidator.Object, _mockedDomainValidator.Object, _mockedRepository.Object);
    }

    [Fact]
    public async Task ValidateAsync_ReturnBusinessFailure_Detail_CallRequestValidator()
    {
        const string validationFailureString = "TestingTesting";

        var request = new Request();

        var validationResult = new ValidationResult(
            new List<ValidationFailure>
            {
                new (string.Empty, validationFailureString)
            });

        _mockedRequestValidator.Setup(e => e.ValidateAsync(request, default))
            .ReturnsAsync(validationResult)
            .Verifiable();

        var handlerResponse = await _handler.HandleAsync(request);

        handlerResponse.IsT1.Should().BeTrue();
        handlerResponse.AsT1
            .Errors.Should().ContainSingle(e => e == validationFailureString);
        handlerResponse.AsT1
            .Kind.Should().Be(FailureKind.Validation);

        _mockedRequestValidator.Verify();
        _mockedRequestValidator.VerifyNoOtherCalls();
        
        _mockedDomainValidator.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ValidateAsync_ReturnBusinessFailure_Detail_CallRequestAndDomainValidators()
    {
        const string validationFailureString = "TestingTesting";

        var request = new Request();
        var successValidationResult = new ValidationResult();
        var failureValidationResult = new ValidationResult(
            new List<ValidationFailure>
            {
                new (string.Empty, validationFailureString)
            });

        _mockedRequestValidator.Setup(e => e.ValidateAsync(request, default))
            .ReturnsAsync(successValidationResult)
            .Verifiable();
        _mockedDomainValidator.Setup(e => e.ValidateAsync(It.IsAny<Domain>(), default))
            .ReturnsAsync(failureValidationResult)
            .Verifiable();

        var handlerResponse = await _handler.HandleAsync(request);

        handlerResponse.IsT1.Should().BeTrue();
        handlerResponse.AsT1
            .Errors.Should().ContainSingle(e => e == validationFailureString);
        handlerResponse.AsT1
            .Kind.Should().Be(FailureKind.Validation);

        _mockedRequestValidator.Verify();
        _mockedRequestValidator.VerifyNoOtherCalls();

        _mockedDomainValidator.Verify();
        _mockedDomainValidator.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ValidateAsync_ReturnResponse()
    {
        var request = new Request();
        var domain = new Domain();

        _mockedRequestValidator.Setup(e => e.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult())
            .Verifiable();
        _mockedDomainValidator.Setup(e => e.ValidateAsync(It.IsAny<Domain>(), default))
            .ReturnsAsync(new ValidationResult())
            .Verifiable();

        _mockedRepository.Setup(e => e.UpdateAsync(It.IsAny<Domain>(), default))
            .ReturnsAsync(domain)
            .Verifiable();

        var handlerResponse = await _handler.HandleAsync(request);

        handlerResponse.IsT0.Should().BeTrue();

        _mockedRequestValidator.Verify();
        _mockedRequestValidator.VerifyNoOtherCalls();

        _mockedDomainValidator.Verify();
        _mockedDomainValidator.VerifyNoOtherCalls();

        _mockedRepository.Verify();
        _mockedRepository.VerifyNoOtherCalls();
    }
}