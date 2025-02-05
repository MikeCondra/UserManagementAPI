using System.Text.Json;

namespace UserManagementAPI
{
    public class UserRepository
    {
        private readonly string _filePath = "user.json";

        public List<User>? GetAllUsers()
        {
            try
            {
                if (!File.Exists(_filePath)) return new List<User>();
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<User>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
                return new List<User>();
            }
        }

        public void DeleteAllUsers()
        {
            try
            {
                File.Delete(_filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the file: {ex.Message}");
            }
        }

        private void SaveUsers(List<User> users)
        {
            try
            {
                var json = JsonSerializer.Serialize(users);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
            }
        }


        public User? GetUser(string username)
        {
            var normalizedUsername = Program.NormalizeUsername(username);
            var users = GetAllUsers();
            if (users == null) return null;
            return users.FirstOrDefault(u => Program.NormalizeUsername(u.Username) == normalizedUsername);
        }

        public int GetCountUsers()
        {
            var users = GetAllUsers();
            return users.Count;
        }

        public void AddUser(User user)
        {
            user.Username = Program.NormalizeUsername(user.Username);
            if (user.Username == "") return; // do not add empty users
            var users = GetAllUsers();
            if (users.Any(u => Program.NormalizeUsername(u.Username) == user.Username)) return; // do not add duplicate users
            users.Add(user);
            SaveUsers(users);
        }

        public void UpdateUser(User updatedUser)
        {
            var normalizedUsername = Program.NormalizeUsername(updatedUser.Username);
            var users = GetAllUsers();
            var user = users.FirstOrDefault(u => Program.NormalizeUsername(u.Username) == normalizedUsername);
            if (user != null)
            {
                user.Details = updatedUser.Details;
                SaveUsers(users);
            }
        }

        public void DeleteUser(string username)
        {
            var normalizedUsername = Program.NormalizeUsername(username);
            var users = GetAllUsers();
            var user = users.FirstOrDefault(u => Program.NormalizeUsername(u.Username) == normalizedUsername);
            if (user != null)
            {
                users.Remove(user);
                SaveUsers(users);
            }
        }
    }
}