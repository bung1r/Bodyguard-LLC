using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerStats
{
    public bool testBool;
}

public class RuntimePlayerStats : PlayerStats
{
    
    public RuntimePlayerStats (PlayerStats playerStats)
    {
        testBool = playerStats.testBool;
    }
}