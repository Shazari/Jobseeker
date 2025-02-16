using FirebaseAdmin.Auth;
using System.Security.Claims;

namespace Jobseeker.Api.Middlewares;

public class FirebaseRoleMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader["Bearer ".Length..];
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                var uid = decodedToken.Uid;

                var user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
                if (user.CustomClaims.TryGetValue("roles", out var rolesObj) && rolesObj is string rolesStr)
                {
                    var roles = rolesStr.Split(',');
                    var claims = roles.Select(role => new Claim(ClaimTypes.Role, role));
                    var identity = new ClaimsIdentity(claims, "Firebase");
                    context.User.AddIdentity(identity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Role extraction failed: {ex.Message}");
            }
        }

        await next(context);
    }
}
