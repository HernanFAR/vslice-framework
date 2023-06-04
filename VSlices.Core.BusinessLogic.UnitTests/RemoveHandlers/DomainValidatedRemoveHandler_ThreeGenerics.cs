using FluentAssertions;
using Moq;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.UnitTests.RemoveHandlers;

public class DomainValidatedRemoveHandler_ThreeGenerics
{
    public record Domain;
    public record Response;
    public record Request : ICommand<Response>;

    private readonly Mock<IRemoveRepository<Domain>> _mockedRepository;
    private readonly Mock<EntityValidatedRemoveHandler<Request, Response, Domain>> _mockedHandler;

    public DomainValidatedRemoveHandler_ThreeGenerics()
    {
        _mockedRepository = new Mock<IRemoveRepository<Domain>>();
        _mockedHandler = new Mock<EntityValidatedRemoveHandler<Request, Response, Domain>>(_mockedRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsync()
    {
        var request = new Request();
        var businessFailure = BusinessFailure.Of.NotFoundResource();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsyncAndGetDomainEntityAsyncAndValidateRequestAsync()
    {
        var request = new Request();
        var domain = new Domain();

        var businessFailure = BusinessFailure.Of.NotFoundResource();
        var success = new Success();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.GetAndProcessEntityAsync(request, default))
            .ReturnsAsync(domain);
        _mockedHandler.Setup(e => e.ValidateEntityAsync(domain, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.GetAndProcessEntityAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateEntityAsync(domain, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsyncAndGetDomainEntityAsyncAndValidateDomainAsyncAndRemoveAsync()
    {
        var request = new Request();
        var domain = new Domain();

        var success = new Success();
        var businessFailure = BusinessFailure.Of.NotFoundResource();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.GetAndProcessEntityAsync(request, default))
            .ReturnsAsync(domain);
        _mockedHandler.Setup(e => e.ValidateEntityAsync(domain, default))
            .ReturnsAsync(success);

        _mockedRepository.Setup(e => e.RemoveAsync(domain, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.GetAndProcessEntityAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateEntityAsync(domain, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.RemoveAsync(domain, default), Times.Once);
        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess()
    {
        var request = new Request();
        var domain = new Domain();
        var response = new Response();

        var success = new Success();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.GetAndProcessEntityAsync(request, default))
            .ReturnsAsync(domain);
        _mockedHandler.Setup(e => e.ValidateEntityAsync(domain, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.GetResponse(domain, request))
            .Returns(response);

        _mockedRepository.Setup(e => e.RemoveAsync(domain, default))
            .ReturnsAsync(domain);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(response);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.GetAndProcessEntityAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateEntityAsync(domain, default), Times.Once);
        _mockedHandler.Verify(e => e.GetResponse(domain, request), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.RemoveAsync(domain, default), Times.Once);
        _mockedRepository.VerifyNoOtherCalls();
    }
}