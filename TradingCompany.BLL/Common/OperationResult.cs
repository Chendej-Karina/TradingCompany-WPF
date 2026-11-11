namespace TradingCompany.BLL.Common;


public class OperationResult
{
    public bool Success { get; protected init; }
    public string? Error { get; protected init; }

    public static OperationResult Ok() => new() { Success = true };
    public static OperationResult Fail(string error) => new() { Success = false, Error = error };
}

public class OperationResult<T> : OperationResult
{
    public T? Value { get; private init; }

    public static OperationResult<T> Ok(T value) => new() { Success = true, Value = value };
    public static new OperationResult<T> Fail(string error) => new() { Success = false, Error = error };
}
