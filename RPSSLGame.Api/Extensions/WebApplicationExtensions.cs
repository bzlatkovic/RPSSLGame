using Microsoft.AspNetCore.Diagnostics;
using RPSSLGame.Api.Constants;
using RPSSLGame.Api.Models;
using Scalar.AspNetCore;

namespace RPSSLGame.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<WebApplication>>();
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                logger.LogError(exception, "Unhandled exception occurred");

                var (statusCode, error) = exception switch
                {
                    HttpRequestException => (StatusCodes.Status503ServiceUnavailable, ErrorMessages.Game.ExternalServiceUnavailable),
                    _ => (StatusCodes.Status500InternalServerError, ErrorMessages.General.UnexpectedError)
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new ErrorResponse(error.Message, error.Code));
            });
        });
    }

    public static void UseCorsPolicy(this WebApplication app)
    {
        var corsPolicy = app.Configuration["Cors:PolicyName"]!;
        app.UseCors(corsPolicy);
    }

    public static void UseScalarApiReference(this WebApplication app)
    {
        app.MapScalarApiReference("/api", options =>
        {
            options.Title = "RPSSL Game API";
            options.Theme = ScalarTheme.DeepSpace;
            options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.Shell, ScalarClient.Curl);
            options.AddPreferredSecuritySchemes("http");
        });
    }
}