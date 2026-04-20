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
    [Min(0)] public int gatherAmount = 10;

    public string GetActionName()
    {
        return resourceType == ResourceType.Wood ? "TALAR" : "MINAR";
    }

    public void FarmResource()
    {
        if (resourceState == ResourceState.Depleted)
        {
            Debug.Log($"<color=orange>[RECURSO]</color> {name} ya estaba agotado. Se ignora la accion.");
            return;
        }

        bool gathered = false;

        switch(resourceType){
            case ResourceType.Wood:
                ForestChunk forest = GetComponent<ForestChunk>();
                if(forest != null)
                {
                    forest.Cut();
                    gathered = true;
                    Debug.Log($"<color=cyan>[RECURSO]</color> Recurso {forest} fue {GetActionName()} + {gatherAmount} Madera.");
                }
                break;
            case ResourceType.Gold:
                GoldChunk gold = GetComponent<GoldChunk>();
                if(gold != null)
                {
                    gold.Mine();
                    gathered = true;
                    Debug.Log($"<color=cyan>[RECURSO]</color> Recurso {gold} fue {GetActionName()} + {gatherAmount} Oro.");
                }
                break;
        }

        if (gathered)
        {
            GameSessionStats.GetOrCreate().RecordGather(resourceType, gatherAmount, name);
        }

        resourceState = ResourceState.Depleted;
    }
}
