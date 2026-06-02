using UnityEngine;

public class BuildingState : MonoBehaviour
{
    public enum State { Normal, Repairing, Gathering, Generating }

    [Header("Current State")]
    public State currentState = State.Normal;

    public string GetStateLabel()
    {
        switch (currentState)
        {
            case State.Repairing:
                return "Reparando";
            case State.Gathering:
                return "Recolectando";
            case State.Generating:
                return "Generando";
            default:
                return "Normal";
        }
    }
}