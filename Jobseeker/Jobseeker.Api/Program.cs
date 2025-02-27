using Jobseeker.Infrastructure;
using Jobseeker.Application;
using Jobseeker.Api.Endpoints;
using Jobseeker.Api.Middlewares;
using Jobseeker.Infrastructure.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddFirebaseAuthentication(builder.Configuration);
builder.Services.AddApplication();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

await DataSeeder.SeedRolesAndAdminUserAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    Log.Information("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    Log.Information("Response {StatusCode}", context.Response.StatusCode);
});

app.UseMiddleware<FirebaseRoleMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Register endpoints
app.MapUserEndpoints();
app.MapJobPostEndpoints();
app.MapJobApplicationEndpoints();
app.MapContactHistoryEndpoints();
app.MapJobSeekerDocumentEndpoints();
app.MapRoleEndpoints();
app.MapAuthEndpoints();

app.Run();