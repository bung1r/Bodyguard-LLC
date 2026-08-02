using System.Collections.Generic;
using UnityEngine;

public class Checkpoint // big chud saving checkpoint
{
    List<EnemyCheckpoint> enemyCheckpoints = new List<EnemyCheckpoint>(); // list of enemies
    MapCheckpoint mapCheckpoint; // map differences (broken stuff, moved boxes, traps, etc.)
    PlayerCheckpoint playerCheckpoint; // stats about the player specifcially
    EmployerCheckpoint employerCheckpoint; // stats about the employer specifically
    float timeSaved;
    public void ReturnByDeath()
    {
        foreach (EnemyCheckpoint enemy in enemyCheckpoints) enemy.ReturnByDeath(timeSaved);
        mapCheckpoint.ReturnByDeath(timeSaved);
        playerCheckpoint.ReturnByDeath(timeSaved);
        employerCheckpoint.ReturnByDeath(timeSaved);
    }
    public Checkpoint()
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
        playerCheckpoint = new PlayerCheckpoint(roundManager.player);
        employerCheckpoint = new EmployerCheckpoint(roundManager.employer);
    }
}