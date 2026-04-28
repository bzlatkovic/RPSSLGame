using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.OpenApi;
using Polly;
using RPSSLGame.Api.Persistence;
using RPSSLGame.Api.Services;

namespace RPSSLGame.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddRandomNumberService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IRandomNumberService, RandomNumberService>(client => { client.BaseAddress = new Uri(configuration["RandomNumberService:Url"]!); })
            .AddResilienceHandler("random-number", pipeline =>
            {
                pipeline.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    Delay = TimeSpan.FromMilliseconds(500)
                });

                pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    FailureRatio = 0.5,
                    MinimumThroughput = 5,
                    BreakDuration = TimeSpan.FromSeconds(30)
                });

                pipeline.AddTimeout(TimeSpan.FromSeconds(5));
            });
    }

    public static void AddCorsPolicy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var corsPolicy = configuration["Cors:PolicyName"]!;
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()!;

        services.AddCors(options =>
        {
            options.AddPolicy(corsPolicy, policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    public static void AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var policyName = configuration["RateLimiting:PolicyName"]!;
        var permitLimit = configuration.GetValue<int>("RateLimiting:PermitLimit");
        var windowSeconds = configuration.GetValue<int>("RateLimiting:WindowSeconds");
        var queueLimit = configuration.GetValue<int>("RateLimiting:QueueLimit");

        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter(policyName, limiterOptions =>
            {
                limiterOptions.PermitLimit = permitLimit;
                limiterOptions.Window = TimeSpan.FromSeconds(windowSeconds);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = queueLimit;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
    }

    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    }

    public static void AddCustomOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Description = "API for RPSSL(rock, paper, scissors, spock, lizard) game.";

                if (document.Tags == null) return Task.CompletedTask;
                var gameTag = document.Tags.FirstOrDefault(t => t.Name == "Game");

                const string gameTagDescription = "Handles all Rock, Paper, Scissors, Lizard, Spock game operations.\n Provides endpoints for retrieving available choices, playing rounds against the computer, and viewing game statistics.";
                if (gameTag is not null)
                    gameTag.Description = gameTagDescription;
                else
                    document.Tags.Add(new OpenApiTag
                    {
                        Name = "Game",
                        Description = gameTagDescription
                    });

                return Task.CompletedTask;
            });
        });
    }
}