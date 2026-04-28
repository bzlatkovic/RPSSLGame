using FluentValidation;
using RPSSLGame.Api.Extensions;
using RPSSLGame.Api.Persistence.Repositories;
using RPSSLGame.Api.Services;
using RPSSLGame.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameRoundRepository, GameRoundRepository>();
builder.Services.AddRandomNumberService(builder.Configuration);
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });
builder.Services.AddValidatorsFromAssembly(typeof(PlayRequestValidator).Assembly);
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddCustomOpenApi();

var app = builder.Build();

await app.ApplyMigrationsAsync();

app.UseGlobalExceptionHandler();
app.UseCorsPolicy();
app.UseRateLimiter();
app.MapOpenApi();
app.UseScalarApiReference();

app.MapControllers();

app.Run();