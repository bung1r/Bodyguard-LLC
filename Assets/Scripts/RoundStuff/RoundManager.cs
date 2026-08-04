
using System;
using System.Collections.Generic;
using UnityEngine;
public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;
    // for now, manually set these 
    public List<EnemyAI> enemies;
    public List<BaseProp> props;
    public List<Objective> objectives;
    public EmployerAI employer;
    public PlayerController player;
    [HideInInspector] public float roundTime = 0f;
    [SerializeField] private int roundSeed;
    public static Action OnNextWave;
    private GameRandom enemySpawnRNG;
    private int waveIndex = 0;
    public List<Wave> waves;
    public GameObject waveManager;
    private WaveManager waveManagerScript;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        employer = FindFirstObjectByType<EmployerAI>();
        player = FindFirstObjectByType<PlayerController>();

        enemySpawnRNG = new GameRandom(roundSeed + 1);
        waveManagerScript = waveManager.GetComponent<WaveManager>();
    }

    void Update()
    {
        roundTime += Time.deltaTime;
        Timer.Instance.SetTimer(roundTime);

        if (waveIndex >= waves.Count) {return;}

        if (roundTime > waves[waveIndex].startTime)
        {
            OnNextWave?.Invoke();
            waveManagerScript.SpawnWave(waves[waveIndex]);
            waveIndex++;
        }
    }
   


    public static GameObject Instantiate(GameObject gameObject)
    {
        return Instantiate(gameObject); 
    }
    
}   