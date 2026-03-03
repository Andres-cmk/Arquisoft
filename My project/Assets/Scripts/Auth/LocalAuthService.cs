using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LocalAuthService : IAuthService
{
    private const string LocalTokenPrefix = "local-offline-token-";
    private readonly UserRepository _repo = new UserRepository();

    public Task<AuthResult> RegisterAsync(string username, string password)
    {
        username = (username ?? "").Trim();
        password = password ?? "";

        if (string.IsNullOrWhiteSpace(username))
            return Task.FromResult(AuthResult.Fail("Usuario requerido."));
        if (password.Length < 6)
            return Task.FromResult(AuthResult.Fail("La contraseña debe tener al menos 6 caracteres."));

        var users = _repo.GetAll();
        bool exists = users.Any(u => string.Equals(u.username, username, StringComparison.OrdinalIgnoreCase));
        if (exists)
            return Task.FromResult(AuthResult.Fail("El usuario ya existe."));

        string salt = PasswordHasher.GenerateSaltBase64();
        string hash = PasswordHasher.HashPassword(password, salt);

        var user = new UserRecord
        {
            username = username,
            salt = salt,
            passwordHash = hash
        };

        users.Add(user);
        _repo.SaveAll(users);

        // Fallback offline: se genera token local sintetico para no romper flujo.
        return Task.FromResult(AuthResult.Ok("Usuario creado.", ToClientUser(user), CreateLocalToken()));
    }

    public Task<AuthResult> LoginAsync(string username, string password)
    {
        username = (username ?? "").Trim();
        password = password ?? "";

        var user = _repo.FindByUsername(username);
        if (user == null)
            return Task.FromResult(AuthResult.Fail("Usuario o contraseña inválidos."));

        bool valid = PasswordHasher.VerifyPassword(password, user.salt, user.passwordHash);
        if (!valid)
            return Task.FromResult(AuthResult.Fail("Usuario o contraseña inválidos."));

        // Fallback offline: se genera token local sintetico para no romper flujo.
        return Task.FromResult(AuthResult.Ok("Login exitoso.", ToClientUser(user), CreateLocalToken()));
    }

    private static string CreateLocalToken()
    {
        return LocalTokenPrefix + Guid.NewGuid().ToString("N");
    }

    private static UserRecord ToClientUser(UserRecord source)
    {
        if (source == null)
            return null;

        return new UserRecord
        {
            username = source.username,
            passwordHash = null,
            salt = null
        };
    }
}
