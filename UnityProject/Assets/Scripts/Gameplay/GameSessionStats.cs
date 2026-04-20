using System;
using UnityEngine;

public class GameSessionStats : MonoBehaviour
{
    [Serializable]
    public class ResourceGatherEvent
    {
        public string resourceType;
        public int amount;
        public int totalWood;
        public int totalGold;
        public float elapsedSeconds;
        public string nodeName;
        public string occurredAtUtc;
    }

    [Serializable]
    public class SessionSnapshot
    {
        public int totalWood;
        public int totalGold;
        public int totalGatherActions;
        public float elapsedSeconds;
        public string startedAtUtc;
    }

    [Serializable]
    public class SessionSummaryPayload
    {
        public int totalWood;
        public int totalGold;
        public int totalGatherActions;
        public float elapsedSeconds;
        public string startedAt;
        public string finishedAt;
    }

    public static GameSessionStats Instance { get; private set; }

    public static event Action<SessionSnapshot> StatsChanged;
    public static event Action<ResourceGatherEvent> ResourceGathered;
    public static event Action<bool> SessionSubmitStateChanged;

    private int totalWood;
    private int totalGold;
    private int totalGatherActions;
    private float sessionStartRealtime;
    private DateTime sessionStartUtc;
    private bool isSubmitting;
    private bool sessionSubmitted;

    public bool IsSubmitting => isSubmitting;
    public bool SessionSubmitted => sessionSubmitted;

    public static GameSessionStats GetOrCreate()
    {
        if (Instance != null)
        {
            return Instance;
        }

        Instance = FindFirstObjectByType<GameSessionStats>();
        if (Instance != null)
        {
            return Instance;
        }

        GameObject go = new GameObject("GameSessionStats");
        Instance = go.AddComponent<GameSessionStats>();
        return Instance;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sessionStartRealtime <= 0f)
        {
            sessionStartRealtime = Time.realtimeSinceStartup;
        }

        if (sessionStartUtc == default)
        {
            sessionStartUtc = DateTime.UtcNow;
        }

        NotifyStatsChanged();
    }

    public void RecordGather(ResourceNode.ResourceType resourceType, int amount, string nodeName)
    {
        int safeAmount = Mathf.Max(0, amount);

        if (resourceType == ResourceNode.ResourceType.Wood)
        {
            totalWood += safeAmount;
        }
        else
        {
            totalGold += safeAmount;
        }

        totalGatherActions++;

        ResourceGatherEvent resourceEvent = new ResourceGatherEvent
        {
            resourceType = resourceType.ToString(),
            amount = safeAmount,
            totalWood = totalWood,
            totalGold = totalGold,
            elapsedSeconds = GetElapsedSeconds(),
            nodeName = string.IsNullOrEmpty(nodeName) ? "Unknown" : nodeName,
            occurredAtUtc = DateTime.UtcNow.ToString("o")
        };

        ResourceGathered?.Invoke(resourceEvent);
        NotifyStatsChanged();
    }

    public float GetElapsedSeconds()
    {
        return Mathf.Max(0f, Time.realtimeSinceStartup - sessionStartRealtime);
    }

    public SessionSnapshot GetSnapshot()
    {
        return new SessionSnapshot
        {
            totalWood = totalWood,
            totalGold = totalGold,
            totalGatherActions = totalGatherActions,
            elapsedSeconds = GetElapsedSeconds(),
            startedAtUtc = sessionStartUtc.ToString("o")
        };
    }

    public string BuildDefaultUiText()
    {
        SessionSnapshot snapshot = GetSnapshot();
        TimeSpan elapsed = TimeSpan.FromSeconds(snapshot.elapsedSeconds);
        string elapsedLabel = elapsed.ToString(@"hh\:mm\:ss");

        return "Madera: " + snapshot.totalWood +
               " | Oro: " + snapshot.totalGold +
               " | Recolecciones: " + snapshot.totalGatherActions +
               " | Tiempo: " + elapsedLabel;
    }

    public SessionSummaryPayload BuildSummaryPayload()
    {
        SessionSnapshot snapshot = GetSnapshot();
        return new SessionSummaryPayload
        {
            totalWood = snapshot.totalWood,
            totalGold = snapshot.totalGold,
            totalGatherActions = snapshot.totalGatherActions,
            elapsedSeconds = snapshot.elapsedSeconds,
            startedAt = snapshot.startedAtUtc,
            finishedAt = DateTime.UtcNow.ToString("o")
        };
    }

    public void FinishAndSendSession(Action<string> onSuccess, Action<string> onError)
    {
        if (sessionSubmitted)
        {
            onError?.Invoke("La sesion ya fue enviada.");
            return;
        }

        if (isSubmitting)
        {
            onError?.Invoke("El envio de sesion ya esta en progreso.");
            return;
        }

        ApiClient apiClient = ApiClient.GetOrCreate();
        if (apiClient == null)
        {
            onError?.Invoke("No se encontro ApiClient.");
            return;
        }

        if (!apiClient.IsAuthenticated)
        {
            onError?.Invoke("Debes iniciar sesion antes de terminar la partida.");
            return;
        }

        isSubmitting = true;
        SessionSubmitStateChanged?.Invoke(true);

        SessionSummaryPayload payload = BuildSummaryPayload();
        apiClient.SendSessionSummary(payload,
            successMessage =>
            {
                isSubmitting = false;
                sessionSubmitted = true;
                SessionSubmitStateChanged?.Invoke(false);
                onSuccess?.Invoke(successMessage);
            },
            errorMessage =>
            {
                isSubmitting = false;
                SessionSubmitStateChanged?.Invoke(false);
                onError?.Invoke(errorMessage);
            });
    }

    public void FinishAndSendSessionFromUI()
    {
        FinishAndSendSession(
            onSuccess: message => Debug.Log("[SESSION] Envio exitoso: " + message),
            onError: error => Debug.LogWarning("[SESSION] Error al enviar sesion: " + error));
    }

    private void NotifyStatsChanged()
    {
        StatsChanged?.Invoke(GetSnapshot());
    }
}