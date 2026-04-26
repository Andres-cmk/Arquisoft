using UnityEngine.Networking;

public static class AuthSession
{
    public static bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);
    public static int UserId { get; private set; }
    public static string Username { get; private set; }
    public static string AccessToken { get; private set; }

    public static void SetAuthenticated(int userId, string username, string accessToken)
    {
        UserId = userId;
        Username = username;
        AccessToken = accessToken;
    }

    public static void Clear()
    {
        UserId = 0;
        Username = null;
        AccessToken = null;
    }

    public static void ApplyAuthorization(UnityWebRequest request)
    {
        if (request == null || string.IsNullOrEmpty(AccessToken))
        {
            return;
        }

        request.SetRequestHeader("Authorization", "Bearer " + AccessToken);
    }
}
