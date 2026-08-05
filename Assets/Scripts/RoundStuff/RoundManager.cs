
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
            waveManagerScript.SpawnWave(waves[waveIndex]);
            OnNextWave?.Invoke();
            waveIndex++;
        }
    }
    
    public void EmitSound(Vector3 position, float decibels)
    {
        int enemyMask = (1 << 7);
        Collider[] enemyColliders = Physics.OverlapSphere(position, decibels, enemyMask);
        HashSet<Transform> enemyHash = new HashSet<Transform>();
        if (decibels > 10f)
        {
            Debug.Log("HELLO???");
        }
        foreach (Collider enemy in enemyColliders)
        {
            // Debug.Log(enemyColliders.Length);
            if (enemyHash.Contains(enemy.transform.root)) continue;

            if (enemy.transform.root.TryGetComponent<EnemyAI>(out var ai))
            {
                ai.ReceiveSound(position, decibels);
                enemyHash.Add(enemy.transform.root);
            }
        }
    }

    public void ClearAndDestroyEnemies()
    {
        foreach (EnemyAI enemy in enemies)
        {
            DestroyImmediate(enemy.gameObject);
        }
        enemies.Clear();
    }
    public static GameObject Instantiate(GameObject gameObject)
    {
        if (gameObject == null)
        {
            Debug.Log("Attempted to Instantiate a null reference");
            return gameObject;
        }
        GameObject gameObj = UnityEngine.Object.Instantiate(gameObject);
        return gameObj; 
    }
    public static void ObjectDestroy(GameObject gameObject)
    {
        Destroy(gameObject);
    }
    // ------ SETTERS -------
    public void SetWaveIndex(int value) => waveIndex = value;
    // ------ GETTERS -------
    public int GetWaveIndex() => waveIndex;
}   