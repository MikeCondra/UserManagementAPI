using System.Text.Json;

namespace UserManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // Add the exception handling middleware
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Other middleware
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            var repository = new UserRepository();

            app.MapGet("/", () => Results.Text("I am root!"));

            app.MapGet("/test", () =>
            {
                repository.DeleteAllUsers();
                var tests = new UserRepositoryTests();
                var list = tests.RunAll();
                return Results.Ok(list);
            });

            app.MapGet("/users", () => Results.Ok(repository.GetAllUsers()));

            app.MapGet("/users/{username}", (string username) =>
            {
                var user = repository.GetUser(username);
                return user != null ? Results.Ok(user) : Results.NotFound();
            });

            app.MapPost("/users", (User user) =>
            {
                repository.AddUser(user);
                return Results.Created($"/users/{user.Username}", user);
            });

            app.MapPut("/users/{username}", (string username, User updatedUser) =>
            {
                if (!UsernameMatch(username, updatedUser.Username)) return Results.BadRequest();
                var existingUser = repository.GetUser(username);
                if (existingUser == null) return Results.NotFound();
                repository.UpdateUser(updatedUser);
                return TypedResults.Ok(updatedUser);
            });

            app.MapDelete("/users/{username}", (string username) =>
            {
                var user = repository.GetUser(username);
                if (user == null) return Results.NotFound();
                repository.DeleteUser(username);
                return Results.NoContent();
            });

            // Endpoint to generate a test exception
            app.MapGet("/generate-exception", () =>
            {
                throw new Exception("This is a test exception!");
            });

            app.MapGet("/logs", () =>
            {
                var logs = RequestResponseLoggingMiddleware.GetLogEntries();
                return Results.Json(logs);
            });

            app.Run();
        }


        public static bool UsernameMatch(string username1, string username2)
        {
            return NormalizeUsername(username1).Equals(NormalizeUsername(username2), StringComparison.OrdinalIgnoreCase);
        }

        public static string NormalizeUsername(string username)
        {
            return username.Trim().ToLowerInvariant();
        }
    }

}