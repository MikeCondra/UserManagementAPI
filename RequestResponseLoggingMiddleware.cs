using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private static ConcurrentQueue<string> _logEntries = new ConcurrentQueue<string>();

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log the request
        var request = await FormatRequest(context.Request);
        Log($"HTTP Request Information:{Environment.NewLine}{request}");

        // Copy a pointer to the original response body stream
        var originalBodyStream = context.Response.Body;

        // Create a new memory stream...
        using var responseBody = new MemoryStream();
        // ...and use that for the temporary response body
        context.Response.Body = responseBody;

        // Continue down the middleware pipeline, eventually returning to this class
        await _next(context);

        // Log the response
        var response = await FormatResponse(context.Response, context.Request.Method);
        Log($"HTTP Response Information:{Environment.NewLine}{response}");

        // Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
        await responseBody.CopyToAsync(originalBodyStream);
    }

    private async Task<string> FormatRequest(HttpRequest request)
    {
        var body = request.Body;

        // Allows using several times the stream in ASP.Net Core
        request.EnableBuffering();

        // Read the stream as text
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        int bytesRead;
        int totalBytesRead = 0;
        while ((bytesRead = await request.Body.ReadAsync(buffer, totalBytesRead, buffer.Length - totalBytesRead)) > 0)
        {
            totalBytesRead += bytesRead;
        }
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        request.Body = body;  // Rewind the stream for the next middleware

        return $"{request.Method} {request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
    }

    private async Task<string> FormatResponse(HttpResponse response, string method)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return $"{method} {response.StatusCode}: {text}";
    }

    private void Log(string logEntry)
    {
        _logEntries.Enqueue(logEntry);
        _logger.LogInformation(logEntry);
    }

    public static ConcurrentQueue<string> GetLogEntries() => _logEntries;
}
