using IVS_API.Models;
using Npgsql;

namespace SpliterX_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database configuration
            var configuration = builder.Configuration;
            var dbConfiguration = configuration.GetSection("DBConfiguration").Get<DBConfiguration>();
            var connectionString = $"Host={dbConfiguration!.Host};Port={dbConfiguration.Port};Username={dbConfiguration.Username};Password={dbConfiguration.Password};Database={dbConfiguration.Database}";
            builder.Services.AddScoped<string>(_ => new string(connectionString));

            // Build the app
            var app = builder.Build();

            // Enable Swagger UI in Development and Production environments
            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Check if running inside Docker (based on environment variable or flag)
            var isRunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

            // Use HTTPS redirection if not in Docker
            if (!isRunningInDocker)
            {
                app.UseHttpsRedirection();
            }

            // Middleware for authorization
            app.UseAuthorization();

            // Map controllers
            app.MapControllers();

            // Set HTTP URL for Docker, HTTPS for local development
           
                app.Urls.Add("http://0.0.0.0:8080");

            // Run the application
            app.Run();
        }
    }
}
