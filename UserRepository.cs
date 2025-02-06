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
                var list = JsonSerializer.Deserialize<List<User>>(json);

                // Normalize username in each incoming User, dropping invalid values
                list.RemoveAll(user =>
                {
                    var name = Program.NormalizeUsername(user.Username);
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        return true; // Remove user if the normalized username is invalid
                    }
                    user.Username = name;
                    return false; // Keep user if the normalized username is valid
                });

                // Detect and remove duplicate usernames
                var seenUsernames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                list.RemoveAll(user =>
                {
                    if (seenUsernames.Contains(user.Username))
                    {
                        return true; // Remove user if the username is a duplicate
                    }
                    seenUsernames.Add(user.Username);
                    return false; // Keep user if the username is unique
                });

                // Do integrity check on incoming List<User>
                return list;
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
            username = Program.NormalizeUsername(username);
            if (string.IsNullOrWhiteSpace(username)) return null; // do not add empty users
            var users = GetAllUsers();
            if (users == null) return null;
            return users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        }

        public int GetCountUsers()
        {
            var users = GetAllUsers();
            if (users == null) return 0;
            return users.Count;
        }

        public void AddUser(User user)
        {
            user.Username = Program.NormalizeUsername(user.Username);
            if (string.IsNullOrWhiteSpace(user.Username)) return; // do not add empty users
            var users = GetAllUsers();
            if (users == null) users = new List<User>();
            if (users.Any(u => string.Equals(u.Username, user.Username, StringComparison.OrdinalIgnoreCase))) return; // do not add duplicate users
            users.Add(user);
            SaveUsers(users);
        }

        public void UpdateUser(User updatedUser)
        {
            updatedUser.Username = Program.NormalizeUsername(updatedUser.Username);
            if (string.IsNullOrWhiteSpace(updatedUser.Username)) return; // do not add empty users
            var users = GetAllUsers();
            if (users == null) return;
            var user = users.FirstOrDefault(u => string.Equals(u.Username, updatedUser.Username, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                user.Details = updatedUser.Details;
                SaveUsers(users);
            }
        }

        public void DeleteUser(string username)
        {
            username = Program.NormalizeUsername(username);
            if (string.IsNullOrWhiteSpace(username)) return;
            var users = GetAllUsers();
            if (users == null) return;
            var user = users.FirstOrDefault(u => Program.NormalizeUsername(u.Username) == username);
            if (user != null)
            {
                users.Remove(user);
                SaveUsers(users);
            }
        }
    }
}