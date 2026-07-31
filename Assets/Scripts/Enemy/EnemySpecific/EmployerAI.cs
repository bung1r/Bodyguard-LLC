using System.Collections.Generic;
using TMPro;
using UnityEngine;

// This is the AI for the employer 
// (AKA the guy you need to protect)

public class EmployerAI : EnemyAI
{
    // THIS IS THE ENUM GUIDE 
    // Idle = Standing still (just normal i guess) (Base)
    // Following = Just follow the pljayer (Base)
    // Chasing = Heading for the next objective position (Override)
    // Searching = Working on an objective  (Override)
    // Frantic = Frantic, run around crazy (Base)

    // REFACTOR THIS LATER!! I WANT THIS TO BE IN A ROUNDMANAGER, THANKS!
    [SerializeField] private List<Transform> objectives = new List<Transform>();
    [SerializeField] private float timePerObjective = 5f;
    private int objectiveIndex = 0;
    

    public override void EnterChasing()
    {
        if (objectiveIndex >= objectives.Count) // have completed all objectives
        {
            SetState(EnemyStates.Idle); // I guess you completed it?
            Debug.Log("Your guy did all the objectives?");
        } 

        agent.isStopped = false;
        agent.speed = baseStats.speed * baseStats.sprintSpeedMult;
        // Transform objective = objectives[objectiveIndex];
        // agent.SetDestination(objective.position + objective.forward * 1.2f); 
    }
    public override void Chasing()
    {
        Transform objective = objectives[objectiveIndex];
        agent.SetDestination(objective.position + objective.forward * 1.2f); 
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
    }
    public override void Searching()
    {
        if (objectiveIndex == objectives.Count)
        {
            SetState(EnemyStates.Idle);
            return;
        }
        RotateManually(objectives[objectiveIndex].position);
        if (Time.time - startedWork > timePerObjective)
        {
            SetState(EnemyStates.Chasing);
            objectiveIndex++;
            return;
        }
    }
    public override void ExitSearching()
    {
        agent.updateRotation = true;
        agent.isStopped = false;
    }
}

