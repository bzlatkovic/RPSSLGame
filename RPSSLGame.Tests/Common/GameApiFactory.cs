using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RPSSLGame.Api.Persistence;
using RPSSLGame.Api.Persistence.Entities;
using RPSSLGame.Api.Services;
using Testcontainers.PostgreSql;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace RPSSLGame.Tests.Common;

public class GameApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16")
        .WithExposedPort(9091)
        .WithDatabase("RPSSLGameTest")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private WireMockServer _wireMockServer;

    public async Task InitializeAsync()
    {
        _wireMockServer = WireMockServer.Start();
        await _postgres.StartAsync();
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        _wireMockServer?.Stop();
        _wireMockServer?.Dispose();
        await _postgres.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IRandomNumberService));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddHttpClient<IRandomNumberService, RandomNumberService>(client => { client.BaseAddress = new Uri($"{_wireMockServer.Url}/random"); });

            var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (dbDescriptor is not null)
                services.Remove(dbDescriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString()));
        });
    }

    public void SetupValidResponse(int randomNumber)
    {
        _wireMockServer?.Reset();

        _wireMockServer?.Given(
            Request.Create().WithPath("/random").UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBody(_ => JsonSerializer.Serialize(new { random_number = randomNumber }))
                .WithHeader("Content-Type", "application/json")
        );
    }

    public void SetupServiceUnavailable()
    {
        _wireMockServer?.Reset();

        _wireMockServer?.Given(
            Request.Create().WithPath("/random").UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(HttpStatusCode.ServiceUnavailable)
                .WithBody("ServiceUnavailable")
                .WithHeader("Content-Type", "text/plain")
        );
    }

    public async Task<IReadOnlyList<GameRound>> GetAllGameRounds()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await dbContext.GameRounds.ToListAsync();
    }

    public async Task AddGameRound(GameRound gameRound)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.GameRounds.AddAsync(gameRound);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddGameRounds(IEnumerable<GameRound> gameRounds)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.GameRounds.AddRangeAsync(gameRounds);
        await dbContext.SaveChangesAsync();
    }
}