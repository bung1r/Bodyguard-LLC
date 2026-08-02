using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCheckpoint : ICheckpointBase
{
    protected GameObject enemyRef;
    protected GameObject enemyPrefab;
    protected Vector3 position;
    protected Quaternion rotation;

    // EnemyAI Variables
    protected EnemyStates currentState;
    protected float lastAttacked; 

    // Nav Mesh Variables
    protected Vector3 destination;
    protected Vector3 velocity;
    protected float speed;
    protected bool isStopped;
    protected bool updateRotation;
    

    // RNG and seed stuff
    protected int seed; // a little unnecessary, but why not?
    protected int wanderRNGNext;

    // Stat Manager and other values
    protected float currentHP;

    public virtual void ReturnByDeath(float timeSaved)
    {
        float timeDifference = Time.time - timeSaved;
        // recreate the enemy reference if it was destroyed after the checkpoint was made
        if (enemyRef == null)
        {
            enemyRef = RoundManager.Instantiate(enemyPrefab);
        }

        EnemyAI enemyAI = enemyRef.GetComponent<EnemyAI>();
        enemyAI.SetLastAttacked(lastAttacked + timeDifference);
        enemyAI.SetStateDirect(currentState);
        enemyAI.SetWanderRNG(wanderRNGNext);

        NavMeshAgent agent = enemyRef.GetComponent<NavMeshAgent>();
        if (agent.enabled)
        {
            agent.Warp(position);
            agent.ResetPath();
            agent.SetDestination(destination);
            agent.speed = speed;
            agent.isStopped = isStopped;
            agent.updateRotation = false;
            enemyRef.transform.rotation = rotation;
            agent.updateRotation = updateRotation;
            agent.velocity = velocity;
        }
        

        enemyAI.GetRuntimeStats().GetBaseStats().currentHealth = currentHP;
    }
    public EnemyCheckpoint(EnemyAI enemyAI)
    {
        enemyRef = enemyAI.gameObject;
        enemyPrefab = enemyAI.prefab;
        position = enemyAI.transform.position;
        rotation = enemyAI.transform.rotation;
        currentState = enemyAI.GetCurrentState();
        destination = enemyAI.GetComponent<NavMeshAgent>().destination;

        NavMeshAgent agent = enemyRef.GetComponent<NavMeshAgent>();
        speed = agent.speed;
        isStopped = agent.isStopped;
        updateRotation = agent.updateRotation;
        velocity = agent.velocity;

        // Seed and RNG stuff
        seed = enemyAI.seed;
        wanderRNGNext = enemyAI.GetWanderRNGCalls();

        currentHP = enemyAI.GetRuntimeStats().GetBaseStats().currentHealth;
        lastAttacked = enemyAI.GetLastAttacked();
        
    }
}