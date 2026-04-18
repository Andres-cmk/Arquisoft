using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum ResourceType
    {
        Wood,
        Gold
    }

    [Header("Resource")]
    public ResourceType resourceType = ResourceType.Wood;

    public string GetActionName()
    {
        return resourceType == ResourceType.Wood ? "TALAR" : "MINAR";
    }
}
