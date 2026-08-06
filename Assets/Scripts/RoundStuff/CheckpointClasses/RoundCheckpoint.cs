public class RoundCheckpoint : ICheckpointBase
{
    float roundTime;
    int waveIndex;
    int objectivesCompleted;
    public void ReturnByDeath(float timeSaved)
    {
        RoundManager roundManager = RoundManager.Instance;
        roundManager.roundTime = roundTime;
        roundManager.SetWaveIndex(waveIndex);
        roundManager.objectivesCompleted = objectivesCompleted;
    }
    public RoundCheckpoint(RoundManager roundManager)
    {
        roundTime = roundManager.roundTime;
        waveIndex = roundManager.GetWaveIndex();
        objectivesCompleted = roundManager.objectivesCompleted;
    }
}