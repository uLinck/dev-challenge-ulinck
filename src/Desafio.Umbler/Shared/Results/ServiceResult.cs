using Desafio.Umbler.Shared.Enum;

namespace Desafio.Umbler.Shared.Results
{
    public class ServiceResult<T>
    {
        public bool Success { get; }
        public T Data { get; }
        public string ErrorMessage { get; }
        public ServiceErrorTypeEnum ErrorType { get; }

        protected ServiceResult(bool success, T data, string errorMessage, ServiceErrorTypeEnum errorType)
        {
            Success = success;
            Data = data;
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        public static ServiceResult<T> Ok(T data) 
            => new ServiceResult<T>(true, data, null, ServiceErrorTypeEnum.None);

        public static ServiceResult<T> ValidationError(string message) 
            => new ServiceResult<T>(false, default, message, ServiceErrorTypeEnum.Validation);

        public static ServiceResult<T> Error(string message) 
            => new ServiceResult<T>(false, default, message, ServiceErrorTypeEnum.ServerError);

        public static ServiceResult<T> NotFound(string message) 
            => new ServiceResult<T>(false, default, message, ServiceErrorTypeEnum.NotFound);
    }
}
