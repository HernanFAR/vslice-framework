using FluentAssertions;
using Moq;
using OneOf.Types;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.UnitTests.CreateHandlers;

public class FullyValidatedCreateHandler_TwoGenerics
{
    public record Domain;
    public record Request;

    private readonly Mock<ICreatableRepository<Domain>> _mockedRepository;
    private readonly Mock<FullyValidatedCreateHandler<Request, Domain>> _mockedHandler;

    public FullyValidatedCreateHandler_TwoGenerics()
    {
        _mockedRepository = new Mock<ICreatableRepository<Domain>>();
        _mockedHandler = new Mock<FullyValidatedCreateHandler<Request, Domain>>(_mockedRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateRequestAsync()
    {
        var request = new Request();
        var businessFailure = BusinessFailure.Of.NotFoundResource();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateRequestAsync(request, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateRequestAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateRequestAsyncAndValidateUseCaseRulesAsync()
    {
        var request = new Request();
        var businessFailure = BusinessFailure.Of.NotFoundResource();
        var success = new Success();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateRequestAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateRequestAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsyncAndValidateDomainAsync()
    {
        var request = new Request();
        var domain = new Domain();

        var businessFailure = BusinessFailure.Of.NotFoundResource();
        var success = new Success();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateRequestAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.GetDomainEntityAsync(request, default))
            .ReturnsAsync(domain);
        _mockedHandler.Setup(e => e.ValidateDomainAsync(domain, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateRequestAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.GetDomainEntityAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateDomainAsync(domain, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsyncAndValidateRequestAsyncAndGetDomainEntityAsyncAndCreateAsync()
    {
        var request = new Request();
        var domain = new Domain();

        var success = new Success();
        var businessFailure = BusinessFailure.Of.NotFoundResource();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateRequestAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.GetDomainEntityAsync(request, default))
            .ReturnsAsync(domain);
        _mockedHandler.Setup(e => e.ValidateDomainAsync(domain, default))
            .ReturnsAsync(success);

        _mockedRepository.Setup(e => e.CreateAsync(domain, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateRequestAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.GetDomainEntityAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateDomainAsync(domain, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.CreateAsync(domain, default), Times.Once);
        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess()
    {
        var request = new Request();
        var domain = new Domain();

        var success = new Success();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateRequestAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.GetDomainEntityAsync(request, default))
            .ReturnsAsync(domain);
        _mockedHandler.Setup(e => e.ValidateDomainAsync(domain, default))
            .ReturnsAsync(success);

        _mockedRepository.Setup(e => e.CreateAsync(domain, default))
            .ReturnsAsync(success);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(success);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateRequestAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.GetDomainEntityAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateDomainAsync(domain, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.CreateAsync(domain, default), Times.Once);
        _mockedRepository.VerifyNoOtherCalls();
    }
}