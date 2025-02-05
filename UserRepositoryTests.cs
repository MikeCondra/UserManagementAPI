using Xunit;

namespace UserManagementAPI
{
    public class UserRepositoryTests
    {
        [Fact]
        public void AddUser_ShouldNormalizeUsername()
        {
            var repository = new UserRepository();
            var user = new UserManagementAPI.User { Username = "  JohnDoe  ", Details = "Details" };
            repository.AddUser(user);
            var retrievedUser = repository.GetUser("johndoe");
            Assert.NotNull(retrievedUser);
            Assert.Equal("Details", retrievedUser.Details);
        }

        // Add more tests for GetUser, UpdateUser, and DeleteUser
    }
}