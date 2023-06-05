namespace VSlices.Core.Abstracts.Responses;

public enum FailureKind
{
    UserNotAllowed,
    NotFoundResource,
    ConcurrencyError,
    ContractValidation,
    DomainValidation
}

public readonly record struct BusinessFailure(FailureKind Kind, string[] Errors)
{
    public static class Of
    {
        public static BusinessFailure NotAllowedUser() => new(FailureKind.UserNotAllowed, Array.Empty<string>());
        public static BusinessFailure NotFoundResource(string[] errors) => new(FailureKind.NotFoundResource, errors);
        public static BusinessFailure NotFoundResource() => new(FailureKind.NotFoundResource, Array.Empty<string>());
        public static BusinessFailure ConcurrencyError(string[] errors) => new(FailureKind.ConcurrencyError, errors);
        public static BusinessFailure ConcurrencyError() => new(FailureKind.ConcurrencyError, Array.Empty<string>());
        public static BusinessFailure ContractValidation(string[] errors) => new(FailureKind.ContractValidation, errors);
        public static BusinessFailure ContractValidation(string error) => new(FailureKind.ContractValidation, new []{ error });
        public static BusinessFailure DomainValidation(string[] errors) => new(FailureKind.DomainValidation, errors);
        public static BusinessFailure DomainValidation(string error) => new(FailureKind.DomainValidation, new []{ error });
    }
}

