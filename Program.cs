using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var repository = new UserRepository();

app.MapGet("/", () =>
{
    return Results.Text("I am root!");
});

app.MapGet("/users", () =>
{
    return Results.Ok(repository.GetAllUsers());
});

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
    if (username != updatedUser.Username) return Results.BadRequest();
    var existingUser = repository.GetUser(username);
    if (existingUser == null) return Results.NotFound();
    repository.UpdateUser(updatedUser);
    return Results.NoContent();
});

app.MapDelete("/users/{username}", (string username) =>
{
    var user = repository.GetUser(username);
    if (user == null) return Results.NotFound();
    repository.DeleteUser(username);
    return Results.NoContent();
});

app.Run();


public class User
{
    public string Username { get; set; }
    public string Details { get; set; }
}


public class UserRepository
{
    private readonly string _filePath = "user.json";

    public List<User> GetAllUsers()
    {
        if (!File.Exists(_filePath)) return new List<User>();
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<User>>(json);
    }

    public User GetUser(string username)
    {
        var users = GetAllUsers();
        return users.FirstOrDefault(u => u.Username == username);
    }

    public void AddUser(User user)
    {
        var users = GetAllUsers();
        users.Add(user);
        SaveUsers(users);
    }

    public void UpdateUser(User updatedUser)
    {
        var users = GetAllUsers();
        var user = users.FirstOrDefault(u => u.Username == updatedUser.Username);
        if (user != null)
        {
            user.Details = updatedUser.Details;
            SaveUsers(users);
        }
    }

    public void DeleteUser(string username)
    {
        var users = GetAllUsers();
        var user = users.FirstOrDefault(u => u.Username == username);
        if (user != null)
        {
            users.Remove(user);
            SaveUsers(users);
        }
    }

    private void SaveUsers(List<User> users)
    {
        var json = JsonSerializer.Serialize(users);
        File.WriteAllText(_filePath, json);
    }
}



