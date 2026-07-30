using System;
using UnityEngine;

[Serializable]
public class BaseStats
{
    public float maxHealth = 100f;
    public float speed = 6f;
    public float sprintSpeedMult = 1.8f;
    public float turnSpeed = 100f;
}

[Serializable]
public class RuntimeBaseStats : BaseStats
{
    public float currentHealth;
    public float currentSpeed;
    public bool isSprinting;
    public bool isWalking;
    public bool isCrouching;
    public RuntimeBaseStats(BaseStats baseStats)
    {
        maxHealth = baseStats.maxHealth;
        speed = baseStats.speed;
        sprintSpeedMult = baseStats.sprintSpeedMult;
        turnSpeed = baseStats.turnSpeed;
        
        currentHealth = baseStats.maxHealth;
        currentSpeed = baseStats.speed;
    }
}
