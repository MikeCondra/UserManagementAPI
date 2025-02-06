# README FOR ACTIVITY 1 


## Creation of the GitHub repo; cloning to local machine

Create a GitHub public repository with “main” as the default branch
    https://github.com/MikeCondra/UserManagementAPI.git

Open a CMD window as administrator.

In the CMD window, CD to c:\coursera    . Make c:\coursera if necessary:
	C:
	cd \
	md coursera
	cd coursera

Run
    git clone https://github.com/MikeCondra/UserManagementAPI.git
to check out the empty project into a new directory, c:\coursera\UserManagementAPI


I added to the project several files to configure GIT and VSCode. They can be seen and taken from the GitHub repro, https://github.com/MikeCondra/UserManagementAPI.git.

    .gitignore -- it excludes the .vscode dir, but allows allows three files under it (settings.json, launch.json, tasks.json) to be included.
    # VS Code
            .vs/
            .vscode-test/
            .vscode/*
            !.vscode/settings.json
            !.vscode/tasks.json
            !.vscode/launch.json

    launch.json -- content to enable debugging
            {
                "version": "0.2.0",
                "configurations": [
                    {
                        "name": ".NET Core Launch (web)",
                        "type": "coreclr",
                        "request": "launch",
                        "preLaunchTask": "build",
                        "program": "${workspaceFolder}/bin/Debug/net9.0/${workspaceFolderBasename}.dll",
                        "args": [],
                        "cwd": "${workspaceFolder}",
                        "stopAtEntry": false,
                        "serverReadyAction": {
                            "action": "openExternally",
                            "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
                        },
                        "env": {
                            "ASPNETCORE_ENVIRONMENT": "Development"
                        },
                        "sourceFileMap": {
                            "/Views": "${workspaceFolder}/Views"
                        },
                        "justMyCode": false,
                        "symbolOptions": {
                            "searchMicrosoftSymbolServer": true,
                            "searchNuGetSymbolServer": true
                        }
                    }
                ]
            }

    tasks.json -- content to enable debugging
            {
                "version": "2.0.0",
                "tasks": [
                    {
                        "label": "build",
                        "command": "dotnet",
                        "type": "process",
                        "args": [
                            "build",
                            "${workspaceFolder}/${workspaceFolderBasename}.csproj",
                            "--configuration",
                            "Debug"
                        ],
                        "problemMatcher": "$msCompile"
                    }
                ]
            }


## Building the skeletal webapi project

CD to c:\coursera

Create a skeletal webapi project:
	Dotnet new webapi -o UserManagementAPI

CD’d to c:\coursera\UserManagementAPI

Ran git commands:
	Git status
	Git add .
	Git commit -m “Initial commit”
	Git push -u origin main
The -u flag on the last command makes main the default, so in the future I can just write “git push”, not “git push origin main”

What the repo looks like on GitHub:  https://gyazo.com/f7f5b3b8fc4c6871544289a5235efd3d	


## Initial running and testing

CD to c:\coursera\UserManagementAPI

Launch VSCode: code .

In program.cs, above the weatherforecast handler, add a root handler:
app.MapGet("/", () => "I am root!");

Open terminal: Type <ctrl>~
In Terminal, run:
	Dotnet clean
	Dotnet restore
	Dotnet build
	Dotnet run
	
In the Terminal log, <ctrl>LeftClick on the link part  of

“Now listening on: http://localhost.xxxx”
Confirm that the browser opens and prints “I am root!”

Stop running by typing <ctrl>C in the Terminal window.



##Testing and Debugging

Put a breakpoint in program.cs at the line you added, app.MapGet("/", () => "I am root!");

Start debugging by clicking on the menu item Run > Start Debugging.

Wait for the symbols to load (the yellow lines in the Debug Console part of the Terminal pane). (This will be faster next time you try it. The first time it has to go get the symbol files.)

The program will stop at your breakpoint in program.cs.

Press F11 to step into app.MapGet.

Press F5 to continue. 

Next time the program stops at your breakpoint, remove it by typing F9, then F5 to continue.


Put everything in git.
	Git status
	Git add .
	Git commit -m “After first build”
	Git push

What it looks like on GitHub: https://gyazo.com/ecf903df2e83806afbac7285a42fbf15



## Using Copilot to implement CRUD operations


I made the decision to use username as ID.


I asked Edge Copilot:

        "I hope you are well. Let's start a new conversation. I am working on a new ASP.NET Core C# project, built form the webapi template, and will be adding inlined APIs in program.cs to implement CRUD operations. The data will be a new User class containing username and details strings. Serialization will be to/from JSON text file user.json.  What do you suggest?”

Edge Copilot reponded with a full replacement for program.cs.

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


I added back a handler for root.

    app.MapGet("/", () =>
    {
        return Results.Text("I am root!");
    });


I disabled nwarnings about nullable reference types with a change in csproj. Changed
	    `<Nullable>enable</Nullable>`
to
	    `<Nullable>disable</Nullable>`

I resolved a build error about JsonSerializer by calling GitHub Copilot, then adding “using System.Text.Json” at the top of program.cs.


## Asking Copilot to generate tests

        "That was excellent; thank you very much. Next, could you add a set of endpoint tests to be dropped into UserManagementAPI.http?"

Copilot responded with an http file.

ManagementAPI_HostAddress = http://localhost:5182

        ### Root endpoint
        GET {{UserManagementAPI_HostAddress}}/
        Accept: application/json

        ### Get all users
        GET {{UserManagementAPI_HostAddress}}/users
        Accept: application/json

        ### Get a user by username
        GET {{UserManagementAPI_HostAddress}}/users/johndoe
        Accept: application/json

        ### Get a user by nonexistent username
        GET {{UserManagementAPI_HostAddress}}/users/bozo
        Accept: application/json

        ### Create a new user
        POST {{UserManagementAPI_HostAddress}}/users
        Content-Type: application/json

        {
        "username": "johndoe",
        "details": "John's details here"
        }

        ### Update a user
        PUT {{UserManagementAPI_HostAddress}}/users/johndoe
        Content-Type: application/json

        {
        "username": "johndoe",
        "details": "Updated details for John"
        }

        ### Delete a user
        DELETE {{UserManagementAPI_HostAddress}}/users/johndoe
        Accept: application/json




Added a “fetch nonexistent user” test and used it.

Saved this document as readme.md

Saved everything to https://github.com/MikeCondra/UserManagementAPI.git
    git status
    git add .
    git commit -m "Project phase 1 complete"
    git push


Project marks the end of Activity 1.




