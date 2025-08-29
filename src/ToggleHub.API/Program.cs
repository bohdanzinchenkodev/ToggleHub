using ToggleHub.API.Extensions;
using ToggleHub.API.Middleware;
using ToggleHub.Application;
using ToggleHub.Infrastructure;
using ToggleHub.Infrastructure.Identity.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAppIdentity(builder.Configuration);
builder.Services.AddApiKeyAuth();

builder.Services.AddApplication();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/openapi/v1.json", "ToggleHub API V1");
    });
}
/*app.UseMiddleware<ExceptionHandlingMiddleware>();*/
app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();