var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddRandomNumberService(builder.Configuration);

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();
app.Run();