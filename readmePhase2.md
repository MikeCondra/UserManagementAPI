//
//
// README FOR ACTIVITY 2: Use of Copilot to improve the code and to debug
//
//


Task 1 was to reenable nullable reference types in the csproj file.
    `<Nullable>disable</Nullable>` => `<Nullable>enable</Nullable>`

Doing this led to compiler warnings, which were resolved in 2 ways: 1) Adding '?' to the end of types that could be assigned a null value, and 2) Adding if() code to do different things when a variable was null. GitHub Copilot and/or VSCode's auto-complete suggested code to do this.

At the start of Activity 2, I manually coded checks to standardize the incoming username capitalization and remove whitespace.  Copilot later suggested a better way, which was to force all incoming names to trimmed lowercase. 

Edge Copilot suggested including an xunit test, and provided one. Getting xunit to run was not particularly hard, but getting the symbols to match up and breakpoints to be hit was harder than expected. I think this is due to version mismatch between the xunit- or maybe dotnet-linked dlls, and those named in the csproj.  There may be an answer to this, so I'll come back to it.  Or, possibly, people using xunit don't routinely Debug the tests.

I found a solution to the xunit problem, which was: 1) remove the Test configuration and all package references to xunit; 2) Disable the [Fact] attribute with a compile-time constant, NOXUNIT; 3) Replace the xunit Assert class with some clone functions in a new Assert.cs; and 4) Modify tests to return a log to a List<string> and eventually show it in a new endpoint, /test. This is what the response to http://localhost:xxxx/test looks like:  


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

Several bugs were found in the UserRepository code by running these tests. (Thanks, Copilot, for suggesting the tests.)

Copilot suggested adding try/catch blocks in places where the serialization file was written/read.


//
//
// TO DO, POSSIBLY AFTER ACTIVITY 2.
//
//

In UserRepositoryTests.cs, add xunit-style tests for delete and put operations.

In Assert.cs, add more clones of xunit Assert class functions. Look up the complete list to see what is available.

Add OpenAPI interface generation.

When other changes are done, try to add back Xunit and its Test configuration in launch.json. At the same time, make the csproj include different packages based on the build type (Debug, Test, Release). That way, I won't break the Debug build when getting Test to work.

