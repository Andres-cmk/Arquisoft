using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionInfoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnitSelection selectionSystem;
    [SerializeField] private GameObject panelInfo;
    [SerializeField] private Slider healthBarUI;
    [SerializeField] private Button generateVillagerButton;
    [SerializeField] private TextMeshProUGUI stateText;

    private GameObject lastShownBuilding;
    private HealthSystem activeHealthSystem;
    private BuildingState activeBuildingState;
    private EdificioCentral activeCentralBuilding;

    void Start()
    {
        if (selectionSystem == null)
        {
            selectionSystem = FindFirstObjectByType<UnitSelection>();
        }

        if (panelInfo != null)
        {
            panelInfo.SetActive(false);
        }

        RegisterSelectionEvents();
        RefreshInfoPanel();
    }

    void OnEnable()
    {
        RegisterSelectionEvents();
        RefreshInfoPanel();
    }

    void OnDisable()
    {
        UnregisterSelectionEvents();
        UnregisterButtonEvents();
    }

    void Update()
    {
        if (panelInfo == null || !panelInfo.activeSelf) return;
        RefreshDynamicValues();
    }

    private void RefreshInfoPanel()
    {
        if (selectionSystem == null || panelInfo == null) return;

        GameObject selectedBuilding = GetFirstSelectedBuilding();
        bool hasBuilding = selectedBuilding != null;

        panelInfo.SetActive(hasBuilding);
        if (!hasBuilding)
        {
            lastShownBuilding = null;
            activeHealthSystem = null;
            activeBuildingState = null;
            activeCentralBuilding = null;
            SetStateText("Normal");
            SetGenerateButtonVisible(false);
            return;
        }

        if (selectedBuilding == lastShownBuilding) return;

        lastShownBuilding = selectedBuilding;
        activeHealthSystem = selectedBuilding.GetComponent<HealthSystem>();
        activeBuildingState = selectedBuilding.GetComponent<BuildingState>();
        activeCentralBuilding = selectedBuilding.GetComponent<EdificioCentral>();

        SetGenerateButtonVisible(activeCentralBuilding != null);
        RegisterButtonEvents();
        RefreshDynamicValues();
    }

    private void RefreshDynamicValues()
    {
        if (activeHealthSystem != null)
        {
            activeHealthSystem.healthBar = healthBarUI;
            activeHealthSystem.ActualizarUI();
        }

        if (activeBuildingState != null)
        {
            SetStateText(activeBuildingState.GetStateLabel());
        }
        else
        {
            SetStateText("Normal");
        }

        if (generateVillagerButton != null && generateVillagerButton.gameObject.activeSelf)
        {
            bool canGenerate = activeCentralBuilding != null && !activeCentralBuilding.estaProduciendo;
            generateVillagerButton.interactable = canGenerate;
        }
    }

    private void RegisterButtonEvents()
    {
        if (generateVillagerButton == null) return;

        generateVillagerButton.onClick.RemoveListener(OnGenerateVillagerClicked);
        generateVillagerButton.onClick.AddListener(OnGenerateVillagerClicked);
    }

    private void UnregisterButtonEvents()
    {
        if (generateVillagerButton == null) return;
        generateVillagerButton.onClick.RemoveListener(OnGenerateVillagerClicked);
    }

    private void OnGenerateVillagerClicked()
    {
        if (activeCentralBuilding == null) return;
        activeCentralBuilding.IniciarProduccion();
        RefreshDynamicValues();
    }

    private void SetGenerateButtonVisible(bool isVisible)
    {
        if (generateVillagerButton == null) return;
        generateVillagerButton.gameObject.SetActive(isVisible);
    }

    private void SetStateText(string value)
    {
        if (stateText == null) return;
        stateText.text = value;
    }

    private void RegisterSelectionEvents()
    {
        if (selectionSystem == null) return;

        selectionSystem.SelectionChanged -= RefreshInfoPanel;
        selectionSystem.SelectionChanged += RefreshInfoPanel;
    }

    private void UnregisterSelectionEvents()
    {
        if (selectionSystem == null) return;
        selectionSystem.SelectionChanged -= RefreshInfoPanel;
    }

    private GameObject GetFirstSelectedBuilding()
    {
        foreach (GameObject selected in selectionSystem.selectedUnits)
        {
            if (selected == null) continue;

            SelectableEntity selectable = SelectionTargetResolver.GetSelectableFromObject(selected);

            if (selectable != null && selectable.Category == SelectableEntity.SelectableCategory.Building)
            {
                return selected;
            }
        }

        return null;
    }
}
