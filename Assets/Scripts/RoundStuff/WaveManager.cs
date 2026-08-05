using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private RoundManager roundManager;
    void Start()
    {
        roundManager = RoundManager.Instance;
    }
    public void SpawnWave(Wave wave)
    {
        foreach (WaveSet waveSet in wave.spawns)
        {
            for (int i=0; i < waveSet.spawnAmount; i++)
            {
                GameObject enemy = Instantiate(waveSet.enemy, waveSet.spawnPosition, Quaternion.identity);
                roundManager.enemies.Add(enemy.GetComponent<EnemyAI>());
            }
        }
    }
}
