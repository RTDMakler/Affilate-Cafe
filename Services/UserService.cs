using Cafe.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace Cafe.Services
{
    // Services/UserService.cs
    public class UserService
    {
        private List<UserModel> users;
        private readonly string filePath;

        public UserService(string filePath)
        {
            this.filePath = filePath;
            LoadData();
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
    }

}
