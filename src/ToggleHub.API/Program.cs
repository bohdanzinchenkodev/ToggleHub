using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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

builder.Logging.ClearProviders();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(x => x.AddService("ToggleHub.Api", serviceVersion: "1.0.0"))
    .WithLogging(logging =>
        {
            logging.AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri("http://localhost:4317");
            });
        })
    .WithMetrics(mb =>
    {
        // Useful built-ins
        mb.AddAspNetCoreInstrumentation();   // request duration, active reqs, etc.
        mb.AddHttpClientInstrumentation();   // outgoing HTTP
        mb.AddRuntimeInstrumentation();      // GC, LOH, exceptions, locks
        mb.AddProcessInstrumentation();      // CPU, mem, threads (optional)

        // Export to collector via OTLP HTTP
        mb.AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri("http://localhost:4317");
        });
    })
    .WithTracing(tb =>
    {
        // Server + client spans
        tb.AddAspNetCoreInstrumentation(opts =>
        {
            opts.RecordException = true;
            opts.Filter = ctx => true; // keep it simple; filter later if needed
        });
        tb.AddHttpClientInstrumentation();

        // Export to Collector (same endpoint you already use)
        tb.AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri("http://localhost:4317");
        });
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