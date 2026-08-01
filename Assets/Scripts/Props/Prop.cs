using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Prop", menuName = "ScriptableObjects/Prop")]
public class Prop : ScriptableObject
{
    public bool physicsProp;

    [Space(3f)]
    public float minDamageVelocity;
    public float damageVelocityMultiplier;

    public DamageType damageType;
}
