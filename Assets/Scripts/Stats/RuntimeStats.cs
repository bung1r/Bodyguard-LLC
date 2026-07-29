using System;

[Serializable]
public class RuntimeStats
{
    public CharacterStats characterStats;
    public RuntimeBaseStats runtimeBaseStats;
    public RuntimePlayerStats runtimePlayerStats;  
    public RuntimeEnemyStats runtimeEnemyStats;
    
    
   
    private void init(BaseStats baseStats, PlayerStats playerStats, EnemyStats enemyStats) 
    {
        if(baseStats != null)
            runtimeBaseStats = new RuntimeBaseStats(baseStats);
        
        if(playerStats != null)
            runtimePlayerStats = new RuntimePlayerStats(playerStats);

        if(enemyStats != null)
            runtimeEnemyStats = new RuntimeEnemyStats(enemyStats);
    }
    public RuntimeStats(BaseStats baseStats, PlayerStats playerStats, EnemyStats enemyStats)
    {
        init(baseStats, playerStats, enemyStats);
    }
    public RuntimeStats(CharacterStats characterStats) 
    {
        init(characterStats.GetBaseStats(), characterStats.GetPlayerStats(), characterStats.GetEnemyStats());
        this.characterStats = characterStats;
    }

}
