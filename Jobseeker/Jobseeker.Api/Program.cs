using Jobseeker.Infrastructure;
using Jobseeker.Application;
using Jobseeker.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapUserEndpoints();
app.MapJobPostEndpoints();
app.MapJobApplicationEndpoints();
app.MapContactHistoryEndpoints();
app.MapJobSeekerDocumentEndpoints();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();