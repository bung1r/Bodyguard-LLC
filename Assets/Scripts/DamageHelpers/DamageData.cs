using System;
using UnityEngine;

[Serializable]
public class DamageData
{
    public DamageType damageType;
    public float damageAmount;
    [HideInInspector] public GameObject source;
    
}

public enum DamageType
{
    Blunt, Pierce, Slash, Other
}