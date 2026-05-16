using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RateLimiterAndAnalytics
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        public RateLimitingMiddleware(RequestDelegate next) { _next = next; }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipaddr = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown ip address.";
            Console.WriteLine($"[GATEKEEPER] Request IP: {ipaddr} |  Time: {DateTime.Now:HH:mm:ss}");
            await _next(context);
        }
    }
}
