using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class StatManager : MonoBehaviour, IDamageable
{
    public CharacterStats characterStats;
    private BaseStats baseStats;
    private PlayerStats playerStats;
    private EnemyStats enemyStats;
    public RuntimeStats runtimeStats;
    void Awake()
    {
        baseStats = characterStats.GetBaseStats();
        playerStats = characterStats.GetPlayerStats();
        enemyStats = characterStats.GetEnemyStats();
        runtimeStats = new RuntimeStats(characterStats);

        

    }
    public void TakeDamage(DamageData damageData)
    {
        Debug.Log(gameObject.name + " took " + damageData.damageAmount + " " + damageData.damageType + " damage from " + damageData.source.name);
        runtimeStats.runtimeBaseStats.currentHealth -= damageData.damageAmount;
        if (runtimeStats.runtimeBaseStats.currentHealth <= 0)
        {
            runtimeStats.runtimeBaseStats.currentHealth = 0;
            Die(damageData);
            // Handle death here (e.g., play animation, disable character, etc.)
        }
        
        // Debug.Log("Please do someting");
    
    
    }
    public void Die(DamageData damageData)
    {
        Debug.Log(gameObject.name + " died from " + damageData.source.name);
        
        Destroy(gameObject, 1f);
        // Handle death here (e.g., play animation, disable character, etc.)
    }


    public RuntimeStats GetRuntimeStats() => runtimeStats;

}

public interface IDamageable
{
    void TakeDamage(DamageData damageData);
    void Die(DamageData damageData);
}