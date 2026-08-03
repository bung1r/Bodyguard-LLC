using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

// This is the AI for the employer 
// (AKA the guy you need to protect)

public class EmployerAI : EnemyAI
{
    // THIS IS THE ENUM GUIDE 
    // Idle = Standing still (just normal i guess) (Base)
    // Following = Just follow the pljayer (Override, only for RotateManually)
    // Chasing = Heading for the next objective position (Override)
    // Searching = Working on an objective  (Override)
    // Frantic = Frantic, run around crazy (Base)

    // REFACTOR THIS LATER!! I WANT THIS TO BE IN A ROUNDMANAGER, THANKS!
    [SerializeField] private List<Objective> objectives;
    [SerializeField] private float timePerObjective = 5f;
    private int objectiveIndex = 0;
    
    public override void Start()
    {
        objectives = RoundManager.Instance.objectives;
        base.Start();
    }
    public override void Following()
    {
        base.Following();
        RotateManually(player.transform.position);
    }
    public override void ExitFollowing()
    {
        base.ExitFollowing();
        agent.updateRotation = true;
    }
    public override void EnterChasing()
    {
        if (objectiveIndex >= objectives.Count) // have completed all objectives
        {
            SetState(EnemyStates.Idle); // I guess you completed it?
            Debug.Log("Your guy did all the objectives?");
        } 

        agent.isStopped = false;
        agent.speed = baseStats.speed * baseStats.sprintSpeedMult;

        agent.ResetPath();
        
    }
    public override void Chasing()
    {
        Objective objective = objectives[objectiveIndex];
        
        agent.SetDestination(objective.transform.position + objective.transform.forward * 1.2f); 

        // Debug.Log($"Remaining: {agent.remainingDistance}");
        // Debug.Log($"Stopping: {agent.stoppingDistance}");
        // Debug.Log($"HasPath: {agent.hasPath}");
        // Debug.Log($"Velocity: {agent.velocity.magnitude}");
        // Debug.Log($"PathStatus: {agent.pathStatus}");
        
        if (ReachedDestination())
        {
            SetState(EnemyStates.Searching);
            return;
        }
    }
    public override void ExitChasing()
    {
        // override so there's nothing. 
    }

    // bro why is this here??
    private float startedWork = -999f;
    public override void EnterSearching()
    {
        startedWork = Time.time;
        agent.isStopped = true;
        agent.speed = 0;
        objectives[objectiveIndex].SetIsWorking(true); // begin working
    }
    public override void Searching()
    {
        if (objectiveIndex == objectives.Count)
        {
            SetState(EnemyStates.Idle);
            return;
        }

        RotateManually(objectives[objectiveIndex].transform.position);

        if (objectives[objectiveIndex].GetIsComplete())
        {
            objectives[objectiveIndex].SetIsWorking(false);
            objectiveIndex++;
            SetState(EnemyStates.Chasing);
            return;
        }

    }
    public override void ExitSearching()
    {
        agent.updateRotation = true;
        agent.isStopped = false;
    }
    // ------- SETTERS --------------
    public void SetStartedWork(float value) => startedWork = value;
    public void SetObjectiveIndex(int value) => objectiveIndex = value;
    // ------- GETTERS --------------
    public float GetStartedWork() => startedWork;
    public int GetObjectiveIndex() => objectiveIndex;
}

