using System;

[Serializable]
public class UserRecord
{
    public string username;
    public string passwordHash;
    public string salt;
}
