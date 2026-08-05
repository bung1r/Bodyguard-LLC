using System.Collections.Generic;
using UnityEngine;

public class Checkpoint // big chud saving checkpoint
{
    List<EnemyCheckpoint> enemyCheckpoints = new List<EnemyCheckpoint>(); // list of enemies
    MapCheckpoint mapCheckpoint; // map differences (broken stuff, moved boxes, traps, etc.)
    PlayerCheckpoint playerCheckpoint; // stats about the player specifcially
    EmployerCheckpoint employerCheckpoint; // stats about the employer specifically
    RoundCheckpoint roundCheckpoint;
    float timeSaved;
    bool isFixed; // fixed checkpoint, type shi
    public void ReturnByDeath()
    {
        
        // RoundManager.Instance.ClearAndDestroyEnemies();
        List<EnemyAI> tempList = new List<EnemyAI>();
        foreach (EnemyCheckpoint enemy in enemyCheckpoints)
        {
            enemy.ReturnByDeath(timeSaved);
            tempList.Add(enemy.enemyRef.GetComponent<EnemyAI>());
        }

        // remove all extra people that may exist. 
        for (int i = RoundManager.Instance.enemies.Count - 1; i >= 0; i--)
        {
            EnemyAI enemyAI = RoundManager.Instance.enemies[i];
            if (!tempList.Contains(enemyAI))
            {
                RoundManager.Instance.enemies.RemoveAt(i);

                if (enemyAI == null || enemyAI.gameObject == null) continue;
                    
                RoundManager.ObjectDestroy(enemyAI.gameObject);
            }
        }
        

        mapCheckpoint.ReturnByDeath(timeSaved);
        playerCheckpoint.ReturnByDeath(timeSaved);
        employerCheckpoint.ReturnByDeath(timeSaved);
        roundCheckpoint.ReturnByDeath(timeSaved);
    }
    public Checkpoint(bool isFixed)
    {
        timeSaved = Time.time;
        RoundManager roundManager = RoundManager.Instance;
        foreach (EnemyAI enemyAI in roundManager.enemies)
        {
            // Debug.Log(enemyAI);
            EnemyCheckpoint enemyCheckpoint = new EnemyCheckpoint(enemyAI);
            // Debug.Log(enemyCheckpoint);
            enemyCheckpoints.Add(enemyCheckpoint);
        }
        mapCheckpoint = new MapCheckpoint(roundManager); 
        playerCheckpoint = new PlayerCheckpoint(roundManager.player, isFixed);
        employerCheckpoint = new EmployerCheckpoint(roundManager.employer);
        roundCheckpoint = new RoundCheckpoint(roundManager);
        this.isFixed = isFixed;
    }
}