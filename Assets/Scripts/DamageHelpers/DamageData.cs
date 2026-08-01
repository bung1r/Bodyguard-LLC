using System;
using UnityEngine;

[Serializable]
public class DamageData
{
    public DamageType damageType;
    public float damageAmount;
    [HideInInspector] public GameObject source;

    // public DamageData(DamageType damageType, float damageAmount, GameObject source)
    // {
    //     this.damageType = damageType;
    //     this.damageAmount = damageAmount;
    //     this.source = source;
    // }
    
}

public enum DamageType
{
    Blunt, Pierce, Slash, Other
}