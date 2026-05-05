using UnityEngine;

public enum RtsEntityKind
{
    None = 0,
    Unit = 1,
    Building = 2,
    Resource = 3
}

public enum RtsUnitType
{
    Unknown = 0,
    Villager = 1,
    Explorer = 2,
    WarriorMelee = 3,
    WarriorDistance = 4
}

public static class RtsUnitTypeUtility
{
    public static RtsUnitType GetUnitType(Humano unit)
    {
        return unit != null ? GetUnitType(unit.gameObject) : RtsUnitType.Unknown;
    }

    public static RtsUnitType GetUnitType(GameObject target)
    {
        if (target == null)
        {
            return RtsUnitType.Unknown;
        }

        if (target.GetComponent<Villager>() != null || target.GetComponentInChildren<Villager>(true) != null)
        {
            return RtsUnitType.Villager;
        }

        if (target.GetComponent<Explorer>() != null || target.GetComponentInChildren<Explorer>(true) != null)
        {
            return RtsUnitType.Explorer;
        }

        if (target.GetComponent<Warrior_Mele>() != null || target.GetComponentInChildren<Warrior_Mele>(true) != null)
        {
            return RtsUnitType.WarriorMelee;
        }

        if (target.GetComponent<Warrior_Distance>() != null || target.GetComponentInChildren<Warrior_Distance>(true) != null)
        {
            return RtsUnitType.WarriorDistance;
        }

        return RtsUnitType.Unknown;
    }

    public static string GetDisplayName(RtsUnitType unitType)
    {
        switch (unitType)
        {
            case RtsUnitType.Villager:
                return "Villager";
            case RtsUnitType.Explorer:
                return "Explorer";
            case RtsUnitType.WarriorMelee:
                return "Warrior_Mele";
            case RtsUnitType.WarriorDistance:
                return "Warrior_Distance";
            default:
                return "Unknown";
        }
    }
}
