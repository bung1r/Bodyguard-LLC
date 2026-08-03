using UnityEngine;

public class ObjectiveCheckpoint : ICheckpointBase
{
    GameObject objectiveRef;
    float progress;
    bool isWorking;
    bool isComplete;
    public void ReturnByDeath(float timeSaved)
    {
        Objective objective = objectiveRef.GetComponent<Objective>();
        objective.SetProgress(progress);
        objective.SetIsWorking(isWorking);
        objective.SetIsComplete(isComplete);
    }
    public ObjectiveCheckpoint(Objective objective)
    {
        objectiveRef = objective.gameObject;
        progress = objective.GetProgress();
        isWorking = objective.GetIsWorking();
        isComplete = objective.GetIsComplete();
    }
}