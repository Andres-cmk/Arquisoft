public static class SessionManager
{
    public static UserRecord CurrentUser { get; private set; }
    public static string AccessToken { get; private set; }

    public static bool IsLoggedIn =>
        CurrentUser != null && !string.IsNullOrEmpty(AccessToken);

    public static void SetSession(UserRecord user, string accessToken)
    {
        CurrentUser = user;
        AccessToken = accessToken;
    }

    public static void Logout()
    {
        CurrentUser = null;
        AccessToken = null;
    }
}
