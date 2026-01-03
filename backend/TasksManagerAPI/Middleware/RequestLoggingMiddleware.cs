using System.Diagnostics;
using System.Text;

namespace TasksManagerAPI.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8];

            // Get client IP address
            var clientIp = GetClientIpAddress(context);

            // Read request body
            var requestBody = await ReadRequestBodyAsync(context.Request);

            _logger.LogInformation(
                "[{RequestId}] Request started: {Method} {Path}{QueryString} - IP: {ClientIp}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                clientIp);

            if (!string.IsNullOrEmpty(requestBody))
            {
                _logger.LogInformation(
                    "[{RequestId}] Request body: {Body}",
                    requestId,
                    requestBody);
            }

            try
            {
                await _next(context);
                stopwatch.Stop();

                _logger.LogInformation(
                    "[{RequestId}] Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms - IP: {ClientIp}",
                    requestId,
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    clientIp);
            }
            catch (Exception)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "[{RequestId}] Request failed: {Method} {Path} - Duration: {Duration}ms - IP: {ClientIp}",
                    requestId,
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    clientIp);
                throw;
            }
        }

        private static string GetClientIpAddress(HttpContext context)
        {
            // Check for forwarded IP (when behind a proxy/load balancer)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // X-Forwarded-For can contain multiple IPs, take the first one (original client)
                return forwardedFor.Split(',')[0].Trim();
            }

            // Check for real IP header (used by some proxies like Nginx)
            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // Fall back to direct connection IP
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            // Only read body for methods that typically have content
            if (request.Method is "GET" or "DELETE" or "HEAD" or "OPTIONS")
            {
                return string.Empty;
            }

            if (request.ContentLength == null || request.ContentLength == 0)
            {
                return string.Empty;
            }

            // Enable buffering so the body can be read multiple times
            request.EnableBuffering();

            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();

            // Reset the stream position for the next middleware/controller
            request.Body.Position = 0;

            return body;
        }
    }
}