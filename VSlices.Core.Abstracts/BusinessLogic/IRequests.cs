using OneOf.Types;

namespace VSlices.Core.Abstracts.BusinessLogic;

public interface IRequest<TResponse> { }

public interface ICommand<TResponse> : IRequest<TResponse> { }

public interface ICommand : ICommand<Success> { }

public interface IQuery<TResponse> : IRequest<TResponse> { }

public interface IQuery : IQuery<Success> { }
