using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

public abstract class EntityFluentValidatedCreateHandler<TRequest, TResponse, TEntity> : EntityValidatedCreateHandler<TRequest, TResponse, TEntity>
    where TRequest : ICommand<TResponse>
{
    private readonly IValidator<TEntity> _entityValidator;

    protected EntityFluentValidatedCreateHandler(IValidator<TEntity> entityValidator, ICreateRepository<TEntity> repository) : base(repository)
    {
        _entityValidator = entityValidator;
    }

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateEntityAsync(TEntity domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _entityValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.DomainValidation(errors);
    }
}

public abstract class EntityFluentValidatedCreateHandler<TRequest, TEntity> : EntityValidatedCreateHandler<TRequest, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TEntity> _entityValidator;

    protected EntityFluentValidatedCreateHandler(IValidator<TEntity> entityValidator, ICreateRepository<TEntity> repository) : base(repository)
    {
        _entityValidator = entityValidator;
    }

    protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateEntityAsync(TEntity domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _entityValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.DomainValidation(errors);
    }
}
