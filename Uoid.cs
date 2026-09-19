namespace ProductsApi;

public readonly union OperationResult<TResult, TError>(TResult, TError);

public readonly struct Uoid;
