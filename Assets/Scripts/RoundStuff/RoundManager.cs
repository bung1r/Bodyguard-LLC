
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
    private GameRandom enemySpawnRNG;
    private int waveIndex = 0;
    private List<float> waves = new List<float>
    {
        20f,40f,60f,80f,
    };
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
        
    }

    void Update()
    {
        roundTime += Time.deltaTime;
        Timer.Instance.SetTimer(roundTime);

        if (roundTime > waves[waveIndex])
        {
            TriggerWave();
            waveIndex++;
        }
    }
    public void TriggerWave()
    {
        player.SetCheckpoint();
    }


    public static GameObject Instantiate(GameObject gameObject)
    {
        return Instantiate(gameObject); 
    }


    
    
}   