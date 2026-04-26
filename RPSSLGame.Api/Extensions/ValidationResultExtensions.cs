using FluentValidation.Results;
using RPSSLGame.Api.Constants;
using RPSSLGame.Api.Models;

namespace RPSSLGame.Api.Extensions;

public static class ValidationResultExtensions
{
    public static ErrorResponse ToErrorResponse(this ValidationResult? validationResult)
    {
        var firstError = validationResult?.Errors?.FirstOrDefault();
        if (firstError is null)
        {
            return new ErrorResponse(
                ErrorMessages.General.UnexpectedError.Message, 
                ErrorMessages.General.UnexpectedError.Code);
        }

        return new ErrorResponse(firstError.ErrorMessage, firstError.ErrorCode);
    }
}