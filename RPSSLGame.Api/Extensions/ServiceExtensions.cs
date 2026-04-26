using Microsoft.Extensions.Http.Resilience;
using Polly;
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

    public static IServiceCollection AddCorsPolicy(
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

        return services;
    }
}