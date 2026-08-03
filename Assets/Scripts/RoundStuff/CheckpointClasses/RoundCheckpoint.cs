public class RoundCheckpoint : ICheckpointBase
{
    float roundTime;
    public void ReturnByDeath(float timeSaved)
    {
        RoundManager.Instance.roundTime = roundTime;
    }
    public RoundCheckpoint(RoundManager roundManager)
    {
        roundTime = roundManager.roundTime;
    }
}