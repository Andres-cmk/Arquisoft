using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UserRepository
{
    private readonly string _filePath;

    public UserRepository()
    {
        _filePath = Path.Combine(Application.persistentDataPath, "users.json");
    }

    public List<UserRecord> GetAll()
    {
        if (!File.Exists(_filePath)) return new List<UserRecord>();

        string json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json)) return new List<UserRecord>();

        UsersFile data = JsonUtility.FromJson<UsersFile>(json);
        return data?.users ?? new List<UserRecord>();
    }

    public void SaveAll(List<UserRecord> users)
    {
        UsersFile data = new UsersFile { users = users };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_filePath, json);
    }

    public UserRecord FindByUsername(string username)
    {
        return GetAll().FirstOrDefault(u =>
            u.username.ToLower() == username.ToLower());
    }
}
