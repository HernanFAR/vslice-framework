using FluentAssertions;
using Moq;
using Moq.Protected;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.UnitTests.RemoveHandlers;

public class RemoveHandler_TwoGenerics
{
    public record Domain;
    public record Request : ICommand;

    private readonly Mock<IRemoveRepository<Domain>> _mockedRepository;
    private readonly Mock<RemoveHandler<Request, Domain>> _mockedHandler;

    public RemoveHandler_TwoGenerics()
    {
        _mockedRepository = new Mock<IRemoveRepository<Domain>>();
        _mockedHandler = new Mock<RemoveHandler<Request, Domain>>(_mockedRepository.Object);
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
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsyncAndGetDomainEntityAsyncAndRemoveAsync()
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

        _mockedRepository.Setup(e => e.RemoveAsync(domain, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.GetAndProcessEntityAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.RemoveAsync(domain, default), Times.Once);
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
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.GetAndProcessEntityAsync(request, default))
            .ReturnsAsync(domain);

        _mockedRepository.Setup(e => e.RemoveAsync(domain, default))
            .ReturnsAsync(domain);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(success);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.GetAndProcessEntityAsync(request, default), Times.Once);
        _mockedHandler.Protected().Verify("GetResponse", Times.Once(), domain, request);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.RemoveAsync(domain, default), Times.Once);
        _mockedRepository.VerifyNoOtherCalls();
    }
}