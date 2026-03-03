using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ApiAuthService : IAuthService
{
    private readonly string _baseUrl;

    public ApiAuthService(string baseUrl)
    {
        _baseUrl = (baseUrl ?? string.Empty).TrimEnd('/');
    }

    public Task<AuthResult> RegisterAsync(string username, string password)
    {
        return PostAuthAsync("/auth/register", username, password);
    }

    public Task<AuthResult> LoginAsync(string username, string password)
    {
        return PostAuthAsync("/auth/login", username, password);
    }

    private async Task<AuthResult> PostAuthAsync(string path, string username, string password)
    {
        if (string.IsNullOrWhiteSpace(_baseUrl))
            return AuthResult.Fail("API base URL no configurada.");

        string url = _baseUrl + path;
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri endpoint))
            return AuthResult.Fail("API base URL invalida.");

        if (!IsSecureEndpoint(endpoint))
            return AuthResult.Fail("La API debe usar HTTPS (HTTP solo permitido en localhost para desarrollo).");

        string payload = JsonUtility.ToJson(new AuthRequestDto
        {
            username = username,
            password = password
        });

        using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = 15;
            request.SetRequestHeader("Content-Type", "application/json");

            var op = request.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();

            string raw = request.downloadHandler != null ? request.downloadHandler.text : string.Empty;
            var dto = TryParseResponse(raw);

            if (request.result != UnityWebRequest.Result.Success)
            {
                if (dto != null && !string.IsNullOrWhiteSpace(dto.message))
                    return AuthResult.Fail(dto.message);

                return AuthResult.Fail("Error de red o servidor: HTTP " + request.responseCode);
            }

            if (dto == null)
                return AuthResult.Fail("Respuesta invalida del servidor.");

            if (!dto.success)
                return AuthResult.Fail(string.IsNullOrWhiteSpace(dto.message) ? "Operacion fallida." : dto.message);

            UserRecord user = null;
            if (dto.user != null)
            {
                user = new UserRecord
                {
                    username = dto.user.username,
                    passwordHash = null,
                    salt = null
                };
            }

            return AuthResult.Ok(
                string.IsNullOrWhiteSpace(dto.message) ? "Operacion exitosa." : dto.message,
                user,
                dto.accessToken,
                dto.refreshToken);
        }
    }

    private static AuthResponseDto TryParseResponse(string rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
            return null;

        try
        {
            return JsonUtility.FromJson<AuthResponseDto>(rawJson);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static bool IsSecureEndpoint(Uri endpoint)
    {
        if (endpoint.Scheme == Uri.UriSchemeHttps)
            return true;

        if (endpoint.Scheme != Uri.UriSchemeHttp)
            return false;

        return string.Equals(endpoint.Host, "localhost", StringComparison.OrdinalIgnoreCase) ||
               endpoint.Host == "127.0.0.1";
    }

    [Serializable]
    private class AuthRequestDto
    {
        public string username;
        public string password;
    }

    [Serializable]
    private class AuthResponseDto
    {
        public bool success;
        public string message;
        public AuthUserDto user;
        public string accessToken;
        public string refreshToken;
    }

    [Serializable]
    private class AuthUserDto
    {
        public int id;
        public string username;
    }
}
