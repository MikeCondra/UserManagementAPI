//#define USEXUNIT

#if NOXUNIT 
#else
using Xunit;
#endif


namespace UserManagementAPI
{

#if NOXUNIT
    // This class collides with the XUnit [Fact] attribute, so we won't build with XUnit
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class FactAttribute : Attribute
    {
        public FactAttribute() { }
    }
#endif

    public class UserRepositoryTests
    {

        public List<string> RunAll()
        {
            List<string> log = new();
            log.Append("Log started.");
            AddUser_Tests(log); // test is implicitly by reference
            // add more tests here
            return log;
        }

        public void AddUser_Tests(List<string> log)
        {
            log.AddRange("~~~ADD USER TESTS~~~");

            // Confirm that capitalization and whitespace are stripped from new users
            {
                log.AddRange("USERNAME CAPITALIZATION AND WHITESPACE STRIPPED");
                var repository = new UserRepository();
                repository.DeleteAllUsers();
                var user = new UserManagementAPI.User { Username = "  JohnDoe  ", Details = "Details" };
                repository.AddUser(user);
                var retrievedUser = repository.GetUser("johndoe");
                log.AddRange(Assert.NotNull(retrievedUser));
                log.AddRange(Assert.Equal("Details", retrievedUser.Details));
            }

            //Submitting an empty username
            {
                log.AddRange("EMPTY USERNAMES REJECTED");
                var repository = new UserRepository();
                repository.DeleteAllUsers();
                var user = new UserManagementAPI.User { Username = "", Details = "Details" };
                repository.AddUser(user);
                log.AddRange(Assert.Equal(repository.GetCountUsers(), 0));
            }
            //Submitting duplicate usernames
            {
                log.AddRange("DUPLICATE USERNAMES NOT ADDED");
                var repository = new UserRepository();
                repository.DeleteAllUsers();
                var user = new UserManagementAPI.User { Username = "JohnDoe", Details = "Details" };
                repository.AddUser(user);
                var user2 = new UserManagementAPI.User { Username = "JohnDoe", Details = "Details" };
                repository.AddUser(user2);
                var retrievedUser = repository.GetUser("johndoe");
                log.AddRange(Assert.NotNull(retrievedUser));
                log.AddRange(Assert.Equal("Details", retrievedUser.Details));
                log.AddRange(Assert.Equal(repository.GetCountUsers(), 1));
            }
        }

        // Add more tests for GetUser, UpdateUser, and DeleteUser
    }
}