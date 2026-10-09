namespace TurnoverSheetService.Services;

public class ServiceResult<T>
{
    public T? ResultObject { get; init; }
    public string? ErrorMessage { get; init; }
    public bool IsSuccess { get; init; }


    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>
        {
            IsSuccess =  true,
            ResultObject = data
        };
    }

    public static ServiceResult<T> Failure(string errorMessage)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}