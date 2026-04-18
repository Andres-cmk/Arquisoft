using System.Collections.Generic;
using System;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public List<GameObject> selectedUnits = new List<GameObject>();
    public event Action SelectionChanged;

    public bool HasSelectedFriendlyUnits()
    {
        foreach (GameObject obj in selectedUnits)
        {
            if (obj == null) continue;

            SelectableEntity selectable = GetSelectable(obj);
            if (selectable != null && selectable.Category == SelectableEntity.SelectableCategory.Unit)
            {
                return true;
            }
        }

        return false;
    }

    public void SelectUnit(GameObject obj)
    {
        if (obj == null) return;

        SelectableEntity selectable = GetSelectable(obj);
        if (selectable == null || !selectable.CanBeSelected) return;

        if (!selectedUnits.Contains(obj))
        {
            selectedUnits.Add(obj);

            selectable.SetSelected(true);
            NotifySelectionChanged();
        }
    }

    public void ClearSelection()
    {
        bool hadSelection = selectedUnits.Count > 0;

        foreach (GameObject obj in selectedUnits)
        {
            if (obj != null)
            {
                SelectableEntity selectable = GetSelectable(obj);
                if (selectable == null) continue;
                selectable.SetSelected(false);
            }
        }
        selectedUnits.Clear();

        if (hadSelection)
        {
            NotifySelectionChanged();
        }
    }

    private SelectableEntity GetSelectable(GameObject obj)
    {
        return SelectionTargetResolver.GetSelectableFromObject(obj);
    }

    private void NotifySelectionChanged()
    {
        SelectionChanged?.Invoke();
    }

}