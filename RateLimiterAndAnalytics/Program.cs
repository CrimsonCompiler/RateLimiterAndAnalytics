using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace RateLimiterAndAnalytics
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

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

            app.Run();

        }
        record StudentRequest(string Name);
    }
}
