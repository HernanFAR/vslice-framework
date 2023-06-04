using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic;

public abstract class CreateHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand<TResponse> 
{
    private readonly ICreateRepository<TEntity> _repository;

    protected CreateHandler(ICreateRepository<TEntity> repository)
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

        var entity = await CreateEntityAsync(request, cancellationToken);

        var dataAccessResult = await _repository.CreateAsync(entity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return GetResponse(entity, request);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<TEntity> CreateEntityAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected internal abstract TResponse GetResponse(TEntity entity, TRequest request);
}

public abstract class EntityValidatedCreateHandler<TRequest, TResponse, TEntity> : IHandler<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly ICreateRepository<TEntity> _repository;

    protected EntityValidatedCreateHandler(ICreateRepository<TEntity> repository)
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

        var entity = await CreateEntityAsync(request, cancellationToken);

        var entityValidationResult = await ValidateEntityAsync(entity, cancellationToken);

        if (entityValidationResult.IsT1)
        {
            return entityValidationResult.AsT1;
        }

        var dataAccessResult = await _repository.CreateAsync(entity, cancellationToken);

        if (dataAccessResult.IsT1)
        {
            return dataAccessResult.AsT1;
        }

        return GetResponse(entity, request);
    }

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<TEntity> CreateEntityAsync(TRequest request,
        CancellationToken cancellationToken = default);

    protected internal abstract ValueTask<OneOf<Success, BusinessFailure>> ValidateEntityAsync(TEntity domain,
        CancellationToken cancellationToken = default);

    protected internal abstract TResponse GetResponse(TEntity entity, TRequest request);
}

public abstract class CreateHandler<TRequest, TEntity> : CreateHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    protected CreateHandler(ICreateRepository<TEntity> repository) : base(repository)
    { }

    protected internal override Success GetResponse(TEntity _, TRequest r) => new();
}

public abstract class EntityValidatedCreateHandler<TRequest, TEntity> : EntityValidatedCreateHandler<TRequest, Success, TEntity>
    where TRequest : ICommand
{
    protected EntityValidatedCreateHandler(ICreateRepository<TEntity> repository) : base(repository)
    { }

    protected internal override Success GetResponse(TEntity _, TRequest r) => new();
}
