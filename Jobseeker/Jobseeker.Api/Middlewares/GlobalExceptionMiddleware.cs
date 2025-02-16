using Serilog;
using System.Net;
using System.Text.Json;

namespace Jobseeker.Api.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var result = JsonSerializer.Serialize(new { message = ex.Message });
        await response.WriteAsync(result);
    }
}