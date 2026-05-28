namespace Catalog.Application.Common;

public enum ResultStatus
{
    Success,
    NotFound,
    Conflict,
    ValidationFailed
}

public class Result
{
    public ResultStatus Status { get; }
    public string? Detail { get; }
    public IDictionary<string, string[]>? Errors { get; }

    protected Result(ResultStatus status, string? detail = null, IDictionary<string, string[]>? errors = null)
    {
        Status = status;
        Detail = detail;
        Errors = errors;
    }

    public bool IsSuccess => Status == ResultStatus.Success;

    public static Result Success() => new(ResultStatus.Success);
    public static Result NotFound(string detail = "Recurso não encontrado.") => new(ResultStatus.NotFound, detail);
    public static Result Conflict(string detail) => new(ResultStatus.Conflict, detail);
    public static Result ValidationFailed(IDictionary<string, string[]> errors) => new(ResultStatus.ValidationFailed, "Erro de validação.", errors);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(ResultStatus.Success)
    {
        Value = value;
    }

    private Result(ResultStatus status, string? detail, IDictionary<string, string[]>? errors)
        : base(status, detail, errors)
    {
    }

    public static Result<T> Success(T value) => new(value);
    public static new Result<T> NotFound(string detail = "Recurso não encontrado.") => new(ResultStatus.NotFound, detail, null);
    public static new Result<T> Conflict(string detail) => new(ResultStatus.Conflict, detail, null);
    public static new Result<T> ValidationFailed(IDictionary<string, string[]> errors) => new(ResultStatus.ValidationFailed, "Erro de validação.", errors);
}
