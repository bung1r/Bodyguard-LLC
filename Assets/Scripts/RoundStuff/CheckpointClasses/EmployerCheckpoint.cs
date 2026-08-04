using UnityEngine;

public class EmployerCheckpoint : EnemyCheckpoint
{
    float startedWork;
    int objectiveIndex;
    public override void ReturnByDeath(float timeSaved)
    {
        base.ReturnByDeath(timeSaved);
        float timeDifference = Time.time - timeSaved;
        enemyRef.GetComponent<EmployerAI>().SetStartedWork(startedWork + timeDifference);
    }
    public EmployerCheckpoint(EmployerAI employerAI) : base(employerAI)
    {
        startedWork = employerAI.GetStartedWork();
        objectiveIndex = employerAI.GetObjectiveIndex();
    }
}