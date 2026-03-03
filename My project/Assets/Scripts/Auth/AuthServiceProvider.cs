using System;

public static class AuthServiceProvider
{
    private static IAuthService _instance;
    private static bool _useApi = true;
    private static string _apiBaseUrl = "http://localhost:3000";

    public static bool IsApiEnabled => _useApi;
    public static string ApiBaseUrl => _apiBaseUrl;

    public static IAuthService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = _useApi
                    ? new ApiAuthService(_apiBaseUrl)
                    : new LocalAuthService();
            }

            return _instance;
        }
    }

    public static void UseLocal()
    {
        _useApi = false;
        _instance = null;
    }

    public static void UseApi(string baseUrl)
    {
        _useApi = true;
        if (!string.IsNullOrWhiteSpace(baseUrl))
            _apiBaseUrl = baseUrl.TrimEnd('/');
        _instance = null;
    }

    public static void SetService(IAuthService service)
    {
        _instance = service;
    }
}
