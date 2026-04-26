namespace RPSSLGame.Api.Constants;

public static class ErrorMessages
{
    public static class Game
    {
        public static readonly (string Code, string Message) ExternalServiceUnavailable = (
            "external_service_unavailable",
            "External service is unavailable. Please try again later");

        public static readonly (string Code, string Message) InvalidChoice = (
            "invalid_choice",
            "Player choice must be between 1 and 5");
    }

    public static class General
    {
        public static readonly (string Code, string Message) UnexpectedError = (
            "unexpected_error",
            "An unexpected error occurred");
    }
}