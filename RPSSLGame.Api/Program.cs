using FluentValidation;
using RPSSLGame.Api.Extensions;
using RPSSLGame.Api.Services;
using RPSSLGame.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddRandomNumberService(builder.Configuration);
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });
builder.Services.AddValidatorsFromAssembly(typeof(PlayRequestValidator).Assembly);
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddRateLimiting(builder.Configuration);

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();
app.UseCorsPolicy();
app.UseRateLimiter();
app.MapOpenApi();
app.MapControllers();

app.Run();