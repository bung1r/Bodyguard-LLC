// This is intended to be deleted once the big S adds the actual RoundManager script. 

using System.Collections.Generic;
using UnityEngine;
public class TempRoundManager : MonoBehaviour
{
    public static TempRoundManager Instance;
    // for now, manually set these 
    public List<EnemyAI> enemies;
    public List<BaseProp> props;
    public EmployerAI employer;
    public PlayerController player;
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
    }

    public static GameObject Instantiate(GameObject gameObject)
    {
        return Instantiate(gameObject);
    }
    
    
}   