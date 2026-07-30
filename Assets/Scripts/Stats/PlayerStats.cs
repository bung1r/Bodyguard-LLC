using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class PlayerStats
{
    public bool playerBool = false;
}

[Serializable]
public class RuntimePlayerStats : PlayerStats
{
    public RuntimePlayerStats(PlayerStats playerStats)
    {
        playerBool = playerStats.playerBool;
    }
}