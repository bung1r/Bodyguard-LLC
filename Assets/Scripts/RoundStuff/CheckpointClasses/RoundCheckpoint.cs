public class RoundCheckpoint : ICheckpointBase
{
    float roundTime;
    int waveIndex;
    public void ReturnByDeath(float timeSaved)
    {
        RoundManager roundManager = RoundManager.Instance;
        roundManager.roundTime = roundTime;
        roundManager.SetWaveIndex(waveIndex);
    }
    public RoundCheckpoint(RoundManager roundManager)
    {
        roundTime = roundManager.roundTime;
        waveIndex = roundManager.GetWaveIndex();
    }
}