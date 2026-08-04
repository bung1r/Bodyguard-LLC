using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public void SpawnWave(Wave wave)
    {
        foreach (WaveSet waveSet in wave.spawns)
        {
            for (int i=0; i < waveSet.spawnAmount; i++)
            {
                Instantiate(waveSet.enemy, waveSet.spawnPosition, Quaternion.identity);
            }
        }
    }
}
