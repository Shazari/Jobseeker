using Jobseeker.Infrastructure;
using Jobseeker.Application;
using Jobseeker.Api.Endpoints;
using Jobseeker.Api.Middlewares;
using Jobseeker.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddFirebaseAuthentication(builder.Configuration);
builder.Services.AddApplication();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

await DataSeeder.SeedRolesAndAdminUserAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<FirebaseRoleMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Register endpoints
app.MapUserEndpoints();
app.MapJobPostEndpoints();
app.MapJobApplicationEndpoints();
app.MapContactHistoryEndpoints();
app.MapJobSeekerDocumentEndpoints();
app.MapRoleEndpoints();
app.MapAuthEndpoints();

app.Run();