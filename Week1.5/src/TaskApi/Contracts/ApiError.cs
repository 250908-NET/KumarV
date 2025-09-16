namespace TaskApi.Contracts;

//might replace this we'll see when the time comes
public class ApiError
{
    public bool Success { get; init; } = false;
    public List<String> Errors { get; init; } = new();
    public string Message { get; init; } = "Operation failed.";
}
