using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    protected StatManager statManager;
    protected Transform player;

    protected virtual void Start()
    {
        // enemy stuff
        statManager = GetComponent<StatManager>();

        // player stuff
        player = FindFirstObjectByType<PlayerController>().transform;
    }

    protected virtual void Update()
    {
        Think();
    }

    public virtual void Think()
    {
        // The AI is thinking, pretty easy to understand. 
        // do not override in most cases
        
    }

    public virtual void Idle()
    {
        
    }
    public virtual void Searching()
    {
        
    }
    public virtual void Chasing()
    {
        
    }
    public virtual void Attack()
    {
        Debug.Log(gameObject.name+" just attacked!");
    }
}

public enum EnemyStates
{
    Idle,
    Searching, 
    Chasing,
    Attacking,
    
}