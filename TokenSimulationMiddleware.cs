using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class TokenSimulationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenSimulationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Query.TryGetValue("simulateToken", out var simulateToken) || simulateToken != "false")
        {
            // If simulateToken is missing or not "false", add the token and proceed to the next middleware
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Request.Headers.Append("Authorization", "valid-token");
            }

            await _next(context);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Not Authorized");
            return;
        }
    }
}
