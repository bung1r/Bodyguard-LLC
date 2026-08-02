// General Behavior:

// Moves slowly, with 2 goons following closely behind
// If see the player, will sprint towards player, then charge (x1.5 sprint speed)
// Deals 2 hearts of damage

using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.LowLevelPhysics2D;

public class BigGoonAI : EnemyAI
{
    protected float maxRushDistance = 10f;
    protected HashSet<Transform> hitHash = new HashSet<Transform>();
    public override void Chasing()
    {
        base.Chasing();
        RotateManually(player.transform.position);
    }
    public override void ExitChasing()
    {
        agent.updateRotation = true;
    }
    public override void EnterAttacking()
    {
        // agent.ResetPath();
        agent.isStopped = false;
        agent.speed = baseStats.speed * baseStats.sprintSpeedMult * 1.7f;

        Vector3 lastValid = transform.position;
        Vector3 direction = transform.forward;

        hitHash.Clear(); // clear the hash

        for (float d = 0.2f; d <= maxRushDistance; d += 0.2f)
        {
            Vector3 point = transform.position + direction * d;

            NavMeshHit hit;
            if (!NavMesh.SamplePosition(point, out hit, 5f, NavMesh.AllAreas))
            {
                Debug.Log("SamplePosition failed at " + d);
                break;
            }

            NavMeshPath path = new NavMeshPath();
            if (!agent.CalculatePath(hit.position, path))
            {
                Debug.Log("CalculatePath failed at " + d);
                break;
            }

            
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                Debug.Log("Path incomplete at " + d);
                break;
            }
                

            lastValid = hit.position;
        }


        agent.SetDestination(lastValid);
    }
    public override void Attacking()
    {
        // Add code for attacking eventually lol.
        if (ReachedDestination())
        {
            SetState(EnemyStates.Chasing);
            return;
        }

        int attackMask = (1 << 6) | (1 << 9); // 
        Collider[] hits = Physics.OverlapBox(
            transform.position, 
            new Vector3(transform.localScale.x * 1.2f, transform.localScale.y * 1.2f, transform.localScale.z * 1.2f), 
            transform.rotation,
            attackMask
        );
        Attack(hits);
        
        if (Time.time - lastAttacked > enemyStats.timeBetweenAttacks)
        {
            lastAttacked = Time.time;
        }
    }
    public override void Attack(){}
    public void Attack(Collider[] hits)
    {
    
        foreach (Collider hit in hits)
        {
            if (hitHash.Contains(hit.transform.root)) continue;
            
            if (hit.transform.TryGetComponent<IDamageable>(out var damageable))
            {
                DamageData damageData = new DamageData {source = gameObject, damageType = DamageType.Blunt, damageAmount = 2f};
                damageable.TakeDamage(damageData);
                hitHash.Add(hit.transform.root);
            }
        }
    }
}