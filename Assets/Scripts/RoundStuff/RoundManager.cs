// This is intended to be deleted once the big S adds the actual RoundManager script. 

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

    [SerializeField] private int roundSeed;
    private GameRandom enemySpawnRNG;
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

    public static GameObject Instantiate(GameObject gameObject)
    {
        return Instantiate(gameObject); 
    }
    
    
}   