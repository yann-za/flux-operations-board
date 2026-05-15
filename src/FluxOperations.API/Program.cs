using FluxOperations.Application;
using FluxOperations.Infrastructure;
using FluxOperations.Infrastructure.Data;
using FluxOperations.Infrastructure.Data.Seeds;
using FluxOperations.API.Middleware;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Flux Operations Board API",
        Version = "v1",
        Description = """
            REST API for the **Flux Operations Board** — a professional operations management
            and supervision platform providing real-time monitoring of data flows, pipelines,
            ETL jobs, API integrations, and streaming processes via an interactive dashboard.
            """,
        Contact = new OpenApiContact
        {
            Name = "Flux Operations Team",
            Email = "ops@fluxboard.io"
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FluxFrontend", policy =>
    {
        var origins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:4200"];

        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flux Operations Board v1");
        c.RoutePrefix = string.Empty;
        c.DocumentTitle = "Flux Operations Board — API";
    });
}

app.UseHttpsRedirection();
app.UseCors("FluxFrontend");
app.UseAuthorization();
app.MapControllers();

// Auto-migrate and seed on startup (dev/staging)
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    await dbContext.Database.EnsureCreatedAsync();
    await DbSeeder.SeedAsync(dbContext, logger);
}

app.Run();

public partial class Program { }
