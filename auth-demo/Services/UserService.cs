using auth_demo.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace auth_demo.Services;
public class UserService
{
    private readonly string _filePath = "users.json";
    private List<User> _users;

    public UserService()
    {
        LoadUsers();
        _users??= new List<User>();
    }

    private void LoadUsers()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            _users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
        }
        else
        {
            _users = new List<User>();
        }
    }

    public Task<User?> FindByUsernameAsync(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username == username);
        return Task.FromResult(user);
    }

    public void AddUser(User user)
    {
        _users.Add(user);
        SaveUsers();
    }

    public void UpdateUser(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Username == user.Username);
        if (existingUser != null)
        {
            existingUser.Password = user.Password;
            SaveUsers();
        }
    }

    private void SaveUsers()
    {
        var json = JsonConvert.SerializeObject(_users, Formatting.Indented);
        File.WriteAllText(_filePath, json);
    }
}
