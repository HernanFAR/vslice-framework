using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Win32.SafeHandles;
using Moq;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.UnitTests.ReadHandlers;

public class ReadHandler_TwoGenerics
{
    public record Request : IQuery<Response>;
    public record Response;

    private readonly Mock<IReadRepository<Response, Request>> _mockedRepository;
    private readonly Mock<ReadHandler<Request, Response>> _mockedHandler;

    public ReadHandler_TwoGenerics()
    {
        _mockedRepository = new Mock<IReadRepository<Response, Request>>();
        _mockedHandler = new Mock<ReadHandler<Request, Response>>(_mockedRepository.Object);
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
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsyncAndRequestToSearchOptionsAsyncAndReadAsync()
    {
        var request = new Request();
        var businessFailure = BusinessFailure.Of.NotFoundResource();
        var success = new Success();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(success);

        _mockedRepository.Setup(e => e.ReadAsync(request, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.ReadAsync(request, default));
        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnResponse()
    {
        var request = new Request();
        var response = new Response();
        var success = new Success();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(success);

        _mockedRepository.Setup(e => e.ReadAsync(request, default))
            .ReturnsAsync(response);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request);

        handlerResponse.Value.Should().Be(response);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.ReadAsync(request, default));
        _mockedRepository.VerifyNoOtherCalls();
    }
}

//public record ItemContract();
//public record BillContract(ItemContract[] Items);
//public record ItemInfo();
//public record BillInfo(ItemInfo[] Items);

//public class A
//{
//    public void XD()
//    {
//        //1.- Leeré el grupo de objetos BillContract uno a uno
//        //2.- Leeré el grupo ItemContract dentro del objeto Bill contract uno a uno
//        //3.- Crearé objeto ItemInfo en base al ItemContract sacado del paso 2
//        //4.- Crearé el objeto BillInfo usando el objeto BillContract sacado del paso 1
//        //    y el grupo de ItemInfos obtenidos del paso 3
//        //5.- Agruparé los objetos Bill info

//        BillContract[] billContracts = null;

//        foreach (var billContract in billContracts)
//        {
//            List<ItemInfo> items;

//            foreach (var itemContract in billContract.Items)
//            {
//                var itemInfo = new ItemInfo(itemContract);

//                items.Add(itemInfo);
//            }

//            var billInfo = new BillInfo(billContract, items);
//        }
//    }
//}