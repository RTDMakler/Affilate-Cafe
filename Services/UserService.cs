using Cafe.Models;
using System.Text.Json;


namespace Cafe.Services
{
    public class UserService
    {
        private const int AdminCode = 1337;
        private List<UserModel> users;
        private readonly string filePath;

        public UserService(string filePath="users.json")
        {
            this.filePath = filePath;
            LoadData();
        }
        public bool IsValidUser(string username, string password)
        {
            var user = users.FirstOrDefault(u => u.UserName == username && u.Password == password);

            return user != null;
        }
        public bool IsAdminCodeValid(int adminCode)
        {
            return adminCode == AdminCode;
        }
        public bool IsAdmin(UserModel user, int adminCode)
        {
            return user.AdminCode == adminCode;
        }

        private void LoadData()
        {
            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                users = JsonSerializer.Deserialize<List<UserModel>>(jsonData) ?? new List<UserModel>();
            }
            else
            {
                users = new List<UserModel>();
            }
        }

        private void SaveData()
        {
            var jsonData = JsonSerializer.Serialize(users);
            File.WriteAllText(filePath, jsonData);
        }

        public void AddUser(UserModel user)
        {
            users.Add(user);
            SaveData();
        }

        public UserModel GetUserByUsername(string username)
        {
            return users.FirstOrDefault(u => u.UserName == username);
        }

        public IEnumerable<UserModel> GetAllUsers()
        {
            return users;
        }

        public void RemoveUser(string requestingUsername, string targetUsername)
        {
            if (requestingUsername != targetUsername)
            {
                var user = GetUserByUsername(targetUsername);
                if (user != null)
                {
                    users.Remove(user);
                    SaveData();
                }
            }
        }
    }

}
