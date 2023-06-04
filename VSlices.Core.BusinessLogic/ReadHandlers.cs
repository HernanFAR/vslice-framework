using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic;

public abstract class ReadHandler<TRequest, TSearchOptions, TResponse> : IHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    private readonly IReadRepository<TResponse, TSearchOptions> _repository;

    protected ReadHandler(IReadRepository<TResponse, TSearchOptions> repository)
    {
        _repository = repository;
    }

    public virtual async ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        var options = await RequestToSearchOptionsAsync(request, cancellationToken);

        return await _repository.ReadAsync(options, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<TSearchOptions> RequestToSearchOptionsAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class ReadHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    private readonly IReadRepository<TResponse, TRequest> _repository;

    protected ReadHandler(IReadRepository<TResponse, TRequest> repository)
    {
        _repository = repository;
    }

    public virtual async ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        return await _repository.ReadAsync(request, cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);
}

public abstract class BasicReadHandler<TRequest, TResponse> : IHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse>
{
    private readonly IReadRepository<TResponse> _repository;

    protected BasicReadHandler(IReadRepository<TResponse> repository)
    {
        _repository = repository;
    }

    public virtual async ValueTask<OneOf<TResponse, BusinessFailure>> HandleAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        var useCaseValidationResult = await ValidateUseCaseRulesAsync(request, cancellationToken);

        if (useCaseValidationResult.IsT1)
        {
            return useCaseValidationResult.AsT1;
        }

        return await _repository.ReadAsync(cancellationToken);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request, CancellationToken cancellationToken = default);
}
