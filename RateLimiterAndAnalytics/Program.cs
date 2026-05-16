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
            app.MapGet("/api/students", () =>
            {
                return students;
            });

                

            app.Run();
        }
    }
}
