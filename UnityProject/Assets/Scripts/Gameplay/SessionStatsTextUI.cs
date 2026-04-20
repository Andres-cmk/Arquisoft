using UnityEngine;
using TMPro;

public class SessionStatsTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private float refreshInterval = 0.25f;

    private float nextRefreshAt;

    private void Awake()
    {
        if (targetText == null)
        {
            targetText = GetComponent<TextMeshProUGUI>();
        }

        GameSessionStats.GetOrCreate();
    }

    private void OnEnable()
    {
        GameSessionStats.StatsChanged += OnStatsChanged;
        RefreshText();
    }

    private void OnDisable()
    {
        GameSessionStats.StatsChanged -= OnStatsChanged;
    }

    private void Update()
    {
        // Keep match time label moving even if no new resource event happened.
        if (Time.unscaledTime >= nextRefreshAt)
        {
            RefreshText();
            nextRefreshAt = Time.unscaledTime + Mathf.Max(0.1f, refreshInterval);
        }
    }

    private void OnStatsChanged(GameSessionStats.SessionSnapshot _)
    {
        RefreshText();
    }

    private void RefreshText()
    {
        if (targetText == null)
        {
            return;
        }

        GameSessionStats stats = GameSessionStats.GetOrCreate();
        targetText.text = stats.BuildDefaultUiText();
    }
}