using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace RateLimiterAndAnalytics
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        // Counting the ip using Dictionary <string, int>
        private static readonly ConcurrentDictionary<string, int> _requestCount = new();

        // Timer: when the timer will hit 0
        private static DateTime _resetTime = DateTime.Now.AddMinutes(1);

        public RateLimitingMiddleware(RequestDelegate next) { _next = next; }




        public async Task InvokeAsync(HttpContext context)
        {

            if(DateTime.Now > _resetTime)
            {
                _requestCount.Clear();
                _resetTime = DateTime.Now.AddMinutes(1);
                Console.WriteLine("[SYSTEM] Rate limit log is cleared");
            }


            var ipaddr = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown ip address.";

            var currentCount = _requestCount.AddOrUpdate(ipaddr, 1, (key, count) => count + 1);

            Console.WriteLine($"[GATEKEEPER] IP: {ipaddr} | Hit Count: {currentCount}");


            if(currentCount > 5)
            {
                Console.WriteLine($"[Blocked] IP {ipaddr} is blocked");
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please wait a minute and try again.");
                return; 
            }
            await _next(context);
        }
    }
}
