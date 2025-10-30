namespace Desafio.Umbler.Shared.Results
{
    public static class Result
    {
        public static ServiceResult<T> Ok<T>(T data) 
            => ServiceResult<T>.Ok(data);

        public static ServiceResult<T> ValidationError<T>(string message) 
            => ServiceResult<T>.ValidationError(message);

        public static ServiceResult<T> Error<T>(string message) 
            => ServiceResult<T>.Error(message);

        public static ServiceResult<T> NotFound<T>(string message) 
            => ServiceResult<T>.NotFound(message);
    }
}
