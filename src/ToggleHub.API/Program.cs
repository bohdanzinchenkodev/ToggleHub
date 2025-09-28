using ToggleHub.API.Extensions;
using ToggleHub.API.Middleware;
using ToggleHub.API.OpenApi;
using ToggleHub.Application.Extensions;
using ToggleHub.Infrastructure.Extensions;
using ToggleHub.Infrastructure.Identity.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<ApiKeySecurityTransformer>();
});


builder.Services.AddControllers();

//ToggleHub 
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAppIdentity(builder.Configuration);
builder.Services.AddApiKeyAuth();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

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
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();