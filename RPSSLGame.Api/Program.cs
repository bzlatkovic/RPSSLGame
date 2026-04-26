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

var app = builder.Build();

app.ConfigureExceptionHandler();
app.MapOpenApi();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();