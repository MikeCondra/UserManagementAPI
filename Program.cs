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
            // test with GET /generate-exception endpoint; be sure in VS to uncheck breakpoint all exceptions
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Add the token authorization token simulation middleware
            // Append ?simulateToken=false to disable simulated authorization
            app.UseMiddleware<TokenSimulationMiddleware>();

            // Request-response logging middleware
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            var repository = new UserRepository();

            app.MapGet("/", () => Results.Text("I am root!"));

            // run simulated xunit tests
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


            // POST /users endpoint
            app.MapPost("/users", async (HttpContext context) =>
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                if (body == null) return Results.BadRequest("No body.");

                var user = JsonSerializer.Deserialize<User>(body);
                if (user == null) return Results.BadRequest("Invalid user data.");
                user.Username = NormalizeUsername(user.Username);
                if (string.IsNullOrWhiteSpace(user.Username)) return Results.BadRequest("Invalid username.");

                // Add user to the repository
                var repository = new UserRepository();
                repository.AddUser(user);
                return TypedResults.Ok(user);
            });

            app.MapPut("/users/{username}", (string username, User updatedUser) =>
            {
                username = Program.NormalizeUsername(username);
                updatedUser.Username = Program.NormalizeUsername(updatedUser.Username);
                if (!string.Equals(username, updatedUser.Username, StringComparison.OrdinalIgnoreCase)) return Results.BadRequest();
                var existingUser = repository.GetUser(username);
                if (existingUser == null) return Results.NotFound();
                repository.UpdateUser(updatedUser);
                return Results.Ok(updatedUser);
            });

            app.MapDelete("/users/{username}", (string username) =>
            {
                var user = repository.GetUser(username); // normalization of username will occur in GetUser
                if (user == null) return Results.NotFound();
                repository.DeleteUser(username);
                return Results.NoContent();
            });

            // Endpoint to generate a test exception
            app.MapGet("/generate-exception", () =>
            {
                throw new Exception("This is a test exception!");
            });

            // Endpoint to simulate an incoming token
            /*             app.MapGet("/simulate-token", (HttpContext context) =>
                        {
                            context.Request.Headers.Append("Authorization", "valid-token");
                            return Results.Text("Token simulated successfully");
                        }); */

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