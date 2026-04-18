using UnityEngine;

public static class SelectionTargetResolver
{
    public struct TargetInfo
    {
        public SelectableEntity selectable;
        public Vector3 worldPoint;
        public int priority;

        public bool HasTarget => selectable != null;
    }

    public static bool TryGetTopSelectable(Camera cam, Vector2 screenPosition, out SelectableEntity selectable, out RaycastHit hit)
    {
        selectable = null;
        hit = default;

        if (cam == null) return false;

        Ray ray = cam.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out hit)) return false;

        selectable = GetSelectableFromCollider(hit.collider);
        return selectable != null;
    }

    public static bool TryGetBestTarget(Camera cam, Vector2 screenPosition, out TargetInfo target)
    {
        target = default;
        if (cam == null) return false;

        Ray ray = cam.ScreenPointToRay(screenPosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        int bestPriority = -1;
        SelectableEntity bestSelectable = null;
        Vector3 bestPoint = Vector3.zero;

        foreach (RaycastHit hit in hits)
        {
            SelectableEntity selectable = GetSelectableFromCollider(hit.collider);
            if (selectable == null) continue;

            int currentPriority = GetPriority(selectable.Category);
            if (currentPriority > bestPriority)
            {
                bestPriority = currentPriority;
                bestSelectable = selectable;
                bestPoint = hit.point;
            }
        }

        if (bestSelectable == null) return false;

        target = new TargetInfo
        {
            selectable = bestSelectable,
            worldPoint = bestPoint,
            priority = bestPriority
        };

        return true;
    }

    public static SelectableEntity GetSelectableFromCollider(Collider collider)
    {
        if (collider == null) return null;
        return collider.GetComponentInParent<SelectableEntity>();
    }

    public static SelectableEntity GetSelectableFromObject(GameObject obj)
    {
        if (obj == null) return null;

        SelectableEntity selectable = obj.GetComponent<SelectableEntity>();
        if (selectable == null)
        {
            selectable = obj.GetComponentInParent<SelectableEntity>();
        }

        return selectable;
    }

    public static int GetPriority(SelectableEntity.SelectableCategory category)
    {
        switch (category)
        {
            case SelectableEntity.SelectableCategory.EnemyUnit:
            case SelectableEntity.SelectableCategory.Unit:
                return 4;
            case SelectableEntity.SelectableCategory.EnemyBuilding:
            case SelectableEntity.SelectableCategory.Building:
            case SelectableEntity.SelectableCategory.Resource:
                return 3;
            case SelectableEntity.SelectableCategory.Ground:
                return 1;
            default:
                return -1;
        }
    }
}
