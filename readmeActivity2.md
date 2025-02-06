# README FOR ACTIVITY 2


## Summary of role of Copilot in debugging and improving functionality

Task 1 was to reenable nullable reference types in the csproj file.
    `<Nullable>disable</Nullable>` => `<Nullable>enable</Nullable>`

Doing this led to compiler warnings, which were resolved manually in 2 ways: 1) Adding '?' to the end of types that could be assigned a null value, and 2) Adding if() code to do different things when a variable was null. GitHub Copilot and/or VSCode's auto-complete suggested code to do this.

I manually coded checks to standardize usernames by removing whitespace and matching names case-insensitively. This allowed username capitalization on a first-come-first-served basis, but not the addition of usernames differing only in capitalization or whitespace from an existing username.

Edge Copilot said, basically, "it works!" It then suggested standardizing the internal representation of usernames on trimmed lowercase.  I replaced the entire program.cs with the Edge Copilot-suggested content.

Edge Copilot also provided an xunit test . Conversations followed to configure the program to build with xunit. 

Copilot generated an http file with several tests for the CRUD operations. I manually added more tests, then added symbol to hold the full host address and used it everywhere.
    @UserManagementAPI_HostAddress = http://localhost:5182

Copilot assisted in turning off "Just My Code" and getting a "portable"" set of symbols for all dlls. This involved changes to launch.json, tasks.json and the csproj.

Copilot also assisted in configuring xunit, but debugging xunit tests wwould not stop on breakpoints. I spent enough ttime talking with Copilots about this to ditch the Test build, and run tests in the Debug build. I removed the xunit-related packages, made a [Fact] attribute and a skeletal Assert class like in xunit, triggered by a compile-time constant NOXUNIT. When I add back xunitlater I'll redo the csproj so the Test and Debug packages are listed separately.

Copilot suggested adding try/catch handlers in places where the serialization file user.json is read and written. The file is read/written before/after each operation, so it is always in sync.

To make the xunit tests work, I added a function to restore the UserRepository to a known state (empty) at the start of each test. Many tests test the existence of a username, so it needs to start out empty.

 Here is what the /test endpoint produces as it runs all available xunit tests. Note, the output of all xunit tests passed upward as a List<string> /test handler, which returns it.

    HTTP/1.1 200 OK
    Connection: close
    Content-Type: application/json; charset=utf-8
    Date: Wed, 05 Feb 2025 23:02:03 GMT
    Server: Kestrel
    Transfer-Encoding: chunked

    [
    "~~~ADD USER TESTS~~~",
    "USERNAME CAPITALIZATION AND WHITESPACE STRIPPED",
    "Assert.NotNull: Object \"UserManagementAPI.User\" succeeded",
    "Assert.Equal \"Details\"==\"Details\" succeeded",
    "EMPTY USERNAMES REJECTED",
    "Assert.Equal \"0\"==\"0\" succeeded",
    "DUPLICATE USERNAMES NOT ADDED",
    "Assert.NotNull: Object \"UserManagementAPI.User\" succeeded",
    "Assert.Equal \"Details\"==\"Details\" succeeded",
    "Assert.Equal \"1\"==\"1\" succeeded"
    ]

I found several bugs manually in the UserRepository code by running these tests under the debugger and following up on AVs. Imo, it is very, very very useful to be able to debug into xunit-style tests.

Copilot also suggested adding try/catch blocks in places where the serialization file was written/read.



## Improvements to reliability, using Copilot suggestions

In all function that add or modify repository data, ensured that usernames (the keys) are always normalized (lower-case, not empty).  

Made change to the repository's GetUsers function to ensure that usernames are unique and valid. Used Copilot-suggested code for this.




## Things to do next


In UserRepositoryTests.cs, add more xunit-style tests for DELETE, PUT and POST operations. 

Maybe, add OpenAPI/Swagger interface generation.


This marks the end of Activity 2.
