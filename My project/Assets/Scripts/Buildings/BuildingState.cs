using UnityEngine;

public class BuildingState : MonoBehaviour
{
    public enum State { Normal, Repairing, Gathering }

    [Header("Current State")]
    public State currentState = State.Normal;
}