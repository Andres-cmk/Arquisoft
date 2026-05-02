using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SessionStatsHUDConnector : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text gatherCountText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text submitStatusText;

    [Header("Button")]
    [SerializeField] private Button finishSessionButton;

    [Header("Navigation")]
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";

    [Header("Format")]
    [SerializeField] private string woodPrefix = "Madera: ";
    [SerializeField] private string goldPrefix = "Oro: ";
    [SerializeField] private string gatherPrefix = "Recolecciones: ";
    [SerializeField] private string timePrefix = "Tiempo: ";
    [SerializeField] private float timeRefreshInterval = 0.25f;

    private float nextTimeRefresh;
    private bool exitingSession;

    private void Awake()
    {
        GameSessionStats.GetOrCreate();
    }

    private void OnEnable()
    {
        GameSessionStats.StatsChanged += OnStatsChanged;
        GameSessionStats.SessionSubmitStateChanged += OnSessionSubmitStateChanged;

        if (finishSessionButton != null)
        {
            finishSessionButton.onClick.RemoveListener(OnFinishSessionClicked);
            finishSessionButton.onClick.AddListener(OnFinishSessionClicked);
        }

        RefreshFromSnapshot(GameSessionStats.GetOrCreate().GetSnapshot());
        RefreshSubmitControls();
    }

    private void OnDisable()
    {
        GameSessionStats.StatsChanged -= OnStatsChanged;
        GameSessionStats.SessionSubmitStateChanged -= OnSessionSubmitStateChanged;

        if (finishSessionButton != null)
        {
            finishSessionButton.onClick.RemoveListener(OnFinishSessionClicked);
        }
    }

    private void Update()
    {
        if (Time.unscaledTime < nextTimeRefresh)
        {
            return;
        }

        nextTimeRefresh = Time.unscaledTime + Mathf.Max(0.1f, timeRefreshInterval);
        RefreshTimeOnly(GameSessionStats.GetOrCreate().GetElapsedSeconds());
    }

    private void OnStatsChanged(GameSessionStats.SessionSnapshot snapshot)
    {
        RefreshFromSnapshot(snapshot);
    }

    private void OnSessionSubmitStateChanged(bool _)
    {
        RefreshSubmitControls();
    }

    private void OnFinishSessionClicked()
    {
        if (exitingSession)
        {
            return;
        }

        exitingSession = true;
        GameSessionStats stats = GameSessionStats.GetOrCreate();
        SetSubmitStatus("Terminando sesion...");
        RefreshSubmitControls();

        if (!stats.SessionSubmitted && !stats.IsSubmitting)
        {
            stats.FinishAndSendSession(
                onSuccess: message =>
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        Debug.Log("[HUD] Respuesta backend: " + message);
                    }
                },
                onError: error =>
                {
                    Debug.LogWarning("[HUD] No se pudo enviar sesion: " + error);
                });
        }

        GoToMainMenu();
    }

    private void GoToMainMenu()
    {
        if (MultiplayerBootstrap.Instance != null)
        {
            MultiplayerBootstrap.Instance.LeaveCurrentMatch();
        }

        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogWarning("[HUD] Nombre de escena principal vacio.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(mainMenuSceneName))
        {
            Debug.LogWarning("[HUD] Escena de menu no encontrada en Build Settings: " + mainMenuSceneName);
            SetSubmitStatus("Sesion enviada, pero no se encontro el menu.");
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void RefreshFromSnapshot(GameSessionStats.SessionSnapshot snapshot)
    {
        if (snapshot == null)
        {
            return;
        }

        if (woodText != null)
        {
            woodText.text = woodPrefix + snapshot.totalWood;
        }

        if (goldText != null)
        {
            goldText.text = goldPrefix + snapshot.totalGold;
        }

        if (gatherCountText != null)
        {
            gatherCountText.text = gatherPrefix + snapshot.totalGatherActions;
        }

        RefreshTimeOnly(snapshot.elapsedSeconds);
    }

    private void RefreshTimeOnly(float elapsedSeconds)
    {
        if (timeText == null)
        {
            return;
        }

        TimeSpan elapsed = TimeSpan.FromSeconds(Mathf.Max(0f, elapsedSeconds));
        timeText.text = timePrefix + elapsed.ToString(@"hh\:mm\:ss");
    }

    private void RefreshSubmitControls()
    {
        GameSessionStats stats = GameSessionStats.GetOrCreate();
        bool disableButton = stats.IsSubmitting || exitingSession;

        if (finishSessionButton != null)
        {
            finishSessionButton.interactable = !disableButton;
        }

        if (stats.SessionSubmitted)
        {
            SetSubmitStatus("Sesion ya enviada.");
        }
        else if (stats.IsSubmitting)
        {
            SetSubmitStatus("Enviando sesion...");
        }
    }

    private void SetSubmitStatus(string value)
    {
        if (submitStatusText != null)
        {
            submitStatusText.text = value;
        }
    }
}
