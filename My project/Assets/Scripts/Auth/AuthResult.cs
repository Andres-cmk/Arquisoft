public class AuthResult
{
    public bool Success;
    public string Message;
    public UserRecord User;
    public string AccessToken;
    public string RefreshToken;

    public static AuthResult Ok(
        string message,
        UserRecord user = null,
        string accessToken = null,
        string refreshToken = null)
        => new AuthResult
        {
            Success = true,
            Message = message,
            User = user,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

    public static AuthResult Fail(string message)
        => new AuthResult
        {
            Success = false,
            Message = message,
            User = null,
            AccessToken = null,
            RefreshToken = null
        };
}
