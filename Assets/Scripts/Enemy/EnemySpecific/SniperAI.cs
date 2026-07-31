
// General Behaviors: 

// Will stick around in small openings, and will not go into the warehouse unless prompted
// Mainly stay still and await for pray to go within the range
// Initial State = Idle. Only uses Idle and Attacking states. 
using UnityEngine;

public class SniperAI : EnemyAI
{
    public override void Idle()
    {
        Debug.Log(visibleEmployers);
        Debug.Log(visiblePlayers);
        if (visibleEmployers.Length > 0 || visiblePlayers.Length > 0)
        {
            Debug.Log("I do be seein players");
            SetState(EnemyStates.Attacking);
            return;
        }
    }
    
    public override void Attacking()
    {
        if (visibleEmployers.Length == 0 && visiblePlayers.Length == 0)
        {
            SetState(EnemyStates.Idle);
            return;
        }

        if (Time.time - lastAttacked > enemyStats.timeBetweenAttacks)
        {
            Attack(new DamageData{source = gameObject, damageAmount = 2f, damageType = DamageType.Pierce}, 0.1f);
            lastAttacked = Time.time;
        }
        
        RotateManually(visibleEmployers.Length > 0 ? visibleEmployers[0].transform.position : visiblePlayers[0].transform.position);
    }
    public override void ExitAttacking()
    {

    }
 }