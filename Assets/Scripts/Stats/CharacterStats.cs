using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public bool isPlayer;
    public bool isEnemy;
    public BaseStats baseStats;
    public PlayerStats playerStats;
    public EnemyStats enemyStats;
    public BaseStats GetBaseStats()
    {
        return baseStats;
    }
    public PlayerStats GetPlayerStats()
    {
        if (isPlayer) return playerStats;
        return null;
    }
    public EnemyStats GetEnemyStats()
    {
        if (isEnemy) return enemyStats;
        return null;
    }
}