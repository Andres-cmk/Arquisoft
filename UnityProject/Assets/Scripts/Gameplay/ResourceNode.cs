using UnityEngine;

public class ResourceNode : MonoBehaviour
{


    public enum ResourceType
    {
        Wood,
        Gold
    }

    public enum ResourceState
    {
        Available,
        Depleted
    }

    [Header("Resource")]
    public ResourceType resourceType = ResourceType.Wood;
    public ResourceState resourceState = ResourceState.Available;

    public string GetActionName()
    {
        return resourceType == ResourceType.Wood ? "TALAR" : "MINAR";
    }

    public void FarmResource()
    {
        switch(resourceType){
            case ResourceType.Wood:
                ForestChunk forest = GetComponent<ForestChunk>();
                if(forest != null)
                {
                    forest.Cut();
                    Debug.Log($"<color=cyan>[RECURSO]</color> Recurso {forest} fue {GetActionName()} + X Cantidad de Madera.");
                }
                break;
            case ResourceType.Gold:
                GoldChunk gold = GetComponent<GoldChunk>();
                if(gold != null)
                {
                    gold.Mine();
                    Debug.Log($"<color=cyan>[RECURSO]</color> Recurso {gold} fue {GetActionName()} + X Cantidad de Oro.");
                }
                break;
        }
        resourceState = ResourceState.Depleted;
    }
}
