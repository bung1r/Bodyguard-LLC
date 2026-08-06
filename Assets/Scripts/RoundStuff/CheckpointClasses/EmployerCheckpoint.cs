using UnityEngine;

public class EmployerCheckpoint : EnemyCheckpoint
{
    float startedWork;
    int objectiveIndex;
    public override void ReturnByDeath(float timeSaved)
    {
        base.ReturnByDeath(timeSaved);
        float timeDifference = Time.time - timeSaved;
        EmployerAI employerAI = enemyRef.GetComponent<EmployerAI>();
        employerAI.SetStartedWork(startedWork + timeDifference);
        employerAI.SetObjectiveIndex(objectiveIndex);
    }
    public EmployerCheckpoint(EmployerAI employerAI) : base(employerAI)
    {
        startedWork = employerAI.GetStartedWork();
        objectiveIndex = employerAI.GetObjectiveIndex();
    }
}