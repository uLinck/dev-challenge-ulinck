using Desafio.Umbler.Shared.Enum;
using Desafio.Umbler.Shared.Results;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Desafio.Umbler.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected ActionResult<ApiResponse<T>> ToResponse<T>(ServiceResult<T> result) 
        {
            if (result.Success)
                return Ok(new ApiResponse<T>(result.Data));

            return result.ErrorType switch
            {
                ServiceErrorTypeEnum.Validation => UnprocessableEntity(new ApiResponse<T>(result.ErrorMessage)),
                ServiceErrorTypeEnum.NotFound => NotFound(new ApiResponse<T>(result.ErrorMessage)),
                _ => StatusCode(500, new ApiResponse<T>(result.ErrorMessage))
            };
        }
    }

    public class ApiResponse<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; }

        [JsonPropertyName("success")]
        public bool Success => Errors == null || !Errors.Any();

        public ApiResponse(T data)
        {
            Data = data;
            Errors = new List<string>();
        }

        public ApiResponse(string error)
        {
            Data = default;
            Errors = new List<string> { error };
        }
    }
}
