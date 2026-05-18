using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace RateLimiterAndAnalytics
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // Middleware
            app.UseMiddleware<RateLimitingMiddleware>();


            List<string> students = new List<string>
            {
                "Tousif","Anika", "Tasrik"
            };


            app.MapGet("/", () => "Hello World!");

            // GET: All students

            app.MapGet("/api/students", () =>
            {
                return students;
            });


            // POST: Post a student data
            app.MapPost("/api/students", (StudentRequest newStudent) =>
            {
                students.Add(newStudent.Name);
                return Results.Ok(students);
            });


            //Analytics Route
            
            app.MapGet("/api/analytics", async () =>
            {
                string connectionString = "Server=DESKTOP-4915MGS\\SQLEXPRESS;Database=ApiAnalyticsDB;Trusted_Connection=True;TrustServerCertificate=True;";

                var dashboardData = new Dictionary<string, object>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM RequestLogs", conn))
                    {
                        dashboardData["TotalRequests"] = await cmd.ExecuteScalarAsync();
                    }

                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM RequestLogs WHERE IsBlocked = 1", conn))
                    {
                        dashboardData["TotalBlocked"] = await cmd.ExecuteScalarAsync();
                    }

                    string topIpQuery = "SELECT TOP 1 IPAddress FROM RequestLogs GROUP BY IPAddress ORDER BY COUNT(*) DESC";
                    using (SqlCommand cmd = new SqlCommand(topIpQuery, conn))
                    {
                        var topIp = await cmd.ExecuteScalarAsync();
                        dashboardData["TopSpammerIP"] = topIp ?? "None";
                    }
                }

                return Results.Ok(dashboardData);
            });


            app.Run();

        }
        record StudentRequest(string Name);
    }
}
