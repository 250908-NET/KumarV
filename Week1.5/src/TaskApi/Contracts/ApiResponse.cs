namespace TaskApi.Contracts;

//might replace this we'll see when the time comes
public class ApiResponse<T>
{
    public bool Success { get; init; } = true;
    public T? Data { get; init; }
    public string Message { get; init; } = "Operation completed successfully.";
}
