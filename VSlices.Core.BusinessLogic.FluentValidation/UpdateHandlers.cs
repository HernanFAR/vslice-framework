using FluentValidation;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation;

public abstract class EntityFluentValidatedUpdateHandler<TRequest, TResponse, TEntity> : EntityValidatedUpdateHandler<TRequest, TResponse, TEntity>
    where TRequest : ICommand<TResponse>
{
    private readonly IValidator<TEntity> _entityValidator;

    protected EntityFluentValidatedUpdateHandler(IValidator<TEntity> entityValidator, IUpdateRepository<TEntity> repository) : base(repository)
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

        return BusinessFailure.Of.Validation(errors);
    }
}

public abstract class EntityFluentValidatedUpdateHandler<TRequest, TEntity> : EntityValidatedUpdateHandler<TRequest, TEntity>
    where TRequest : ICommand
{
    private readonly IValidator<TEntity> _entityValidator;

    protected EntityFluentValidatedUpdateHandler(IValidator<TEntity> entityValidator, IUpdateRepository<TEntity> repository) : base(repository)
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

        return BusinessFailure.Of.Validation(errors);
    }
}
