using OneOf;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.Sender;

public interface ISender
{
    ValueTask<OneOf<TResponse, BusinessFailure>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

}
