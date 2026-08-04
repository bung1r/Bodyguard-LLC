using System.Collections.Generic;
using UnityEngine;

public class MapCheckpoint : ICheckpointBase
{
    List<PropCheckpoint> propCheckpoints = new List<PropCheckpoint>();
    List<DoorCheckpoint> doorCheckpoints = new List<DoorCheckpoint>();
    List<ObjectiveCheckpoint> objectiveCheckpoints = new List<ObjectiveCheckpoint>();
    
    public void ReturnByDeath(float timeSaved)
    {
        foreach (PropCheckpoint propCheckpoint in propCheckpoints) 
            propCheckpoint.ReturnByDeath(timeSaved);
        
        foreach (DoorCheckpoint doorCheckpoint in doorCheckpoints)
            doorCheckpoint.ReturnByDeath(timeSaved);

        foreach (ObjectiveCheckpoint objectiveCheckpoint in objectiveCheckpoints)
            objectiveCheckpoint.ReturnByDeath(timeSaved);
    }
    public MapCheckpoint(RoundManager roundManager)
    {
        foreach (BaseProp baseProp in roundManager.props)
            propCheckpoints.Add(new PropCheckpoint(baseProp));

        // foreach (Door door in RoundManager.doors) 
        //     doorCheckpoints.Add(new DoorCheckpoint(door));

        foreach (Objective objective in roundManager.objectives)
            objectiveCheckpoints.Add(new ObjectiveCheckpoint(objective));


        
    }
}