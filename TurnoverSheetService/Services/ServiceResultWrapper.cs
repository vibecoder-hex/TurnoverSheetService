namespace TurnoverSheetService.Services;

public class ServiceResult<T>
{
    private T? _resultObject;
    private string? _errorMessage;
    private bool _isSuccess;
    

    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>
        {
            _isSuccess =  true,
            _resultObject = data
        };
    }

    public static ServiceResult<T> Failure(string errorMessage)
    {
        return new ServiceResult<T>
        {
            _isSuccess = false,
            _errorMessage = errorMessage
        };
    }
}