using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace RateLimiterAndAnalytics
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        // Counting the ip using Dictionary <string, int>
        private static readonly ConcurrentDictionary<string, int> _requestCount = new();

        // Timer: when the timer will hit 0
        private static DateTime _resetTime = DateTime.Now.AddMinutes(1);


        private readonly string _connectString = "Server=DESKTOP-4915MGS\\SQLEXPRESS;Database=ApiAnalyticsDB;Trusted_Connection=True;TrustServerCertificate=True;";

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

            bool isBlocked = currentCount > 5;

            // Sending log to database in background
            _ = LogRequestToDatabaseAsync(ipaddr, isBlocked);


            if(isBlocked)
            {
                Console.WriteLine($"[Blocked] IP {ipaddr} is blocked");
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please wait a minute and try again.");
                return; 
            }
            await _next(context);

        }

        private async Task LogRequestToDatabaseAsync(string ip, bool isBlocked)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectString))
                {
                   await conn.OpenAsync();

                    // INSERT COMMAND
                    string query = "INSERT INTO RequestLogs(IPAddress, IsBlocked) VALUES(@ip, @blocked)";

                    using(SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ip", ip);
                        cmd.Parameters.AddWithValue("@blocked", isBlocked);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] Log save failed: {ex.Message}");
            }
        }
    }
}
