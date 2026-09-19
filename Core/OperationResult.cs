namespace ProductsApi.Core;

public readonly union OperationResult<TResult, TError>(TResult, TError);
