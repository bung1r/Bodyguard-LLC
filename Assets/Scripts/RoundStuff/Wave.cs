using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveSet
{
    public GameObject enemy;
    public int spawnAmount;
    public Vector3 spawnPosition;
}

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/Wave")]
public class Wave : ScriptableObject
{
    public float startTime;

    [Space(3f)]

    [SerializeField] public List<WaveSet> spawns;
}
