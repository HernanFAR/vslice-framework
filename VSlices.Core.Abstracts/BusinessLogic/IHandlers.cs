﻿using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Abstracts.BusinessLogic;

/// <summary>
/// Defines a handler for a <see cref="IRequest{TResponse}"/>
/// </summary>
/// <typeparam name="TRequest">The request to be handled</typeparam>
/// <typeparam name="TResponse">The expected response of the handler</typeparam>
public interface IHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request
    /// </summary>
    /// <param name="request">The request to be handled</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    ValueTask<Response<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a handler for a <see cref="IRequest"/>
/// </summary>
/// <typeparam name="TRequest">The request to be handled</typeparam>
public interface IHandler<in TRequest> : IHandler<TRequest, Success>
    where TRequest : IRequest<Success>
{ }

/// <summary>
/// A delegate that represents the next action in the pipeline
/// </summary>
/// <typeparam name="TResponse">The response of the next action</typeparam>
/// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the next action </returns>
public delegate ValueTask<Response<TResponse>> RequestHandlerDelegate<TResponse>();

/// <summary>
/// A middleware behavior for a <see cref="IRequest{TResponse}"/>
/// </summary>
/// <typeparam name="TRequest">The request to intercept</typeparam>
/// <typeparam name="TResponse">The expected response</typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// A method that intercepts the pipeline
    /// </summary>
    /// <param name="request">The intercepted request</param>
    /// <param name="next">The next action</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A <see cref="ValueTask{T}"/> holding a <see cref="Response{TRequest}"/> of <see cref="Success"/> that represents the result of the operation </returns>
    ValueTask<Response<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default);
}