using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAI : MonoBehaviour
{
    protected StatManager statManager;
    protected Transform player;
    protected NavMeshAgent agent;
    protected RuntimeStats runtimeStats;
    protected RuntimeBaseStats baseStats;
    protected RuntimeEnemyStats enemyStats;
    [SerializeField] protected EnemyStates currentState;
    protected EnemyStates prevState; // do NOT change this manually
    protected virtual void Start()
    {
        // enemy stuff
        statManager = GetComponent<StatManager>();
        agent = GetComponent<NavMeshAgent>();

        runtimeStats = statManager.GetRuntimeStats();
        baseStats = runtimeStats.GetBaseStats();
        enemyStats = runtimeStats.GetEnemyStats();

        // player stuff
        player = FindFirstObjectByType<PlayerController>().transform;
        Debug.Log(player);

        // initializing enemy variables
        agent.speed = baseStats.speed;
    }

    protected virtual void Update()
    {
        Think();
    }
    public EnemyStates GetState() => currentState;
    public virtual void SetState(EnemyStates enemyState)
    {
        currentState = enemyState;
    }
    public virtual void Think()
    {
        // The AI is thinking, pretty easy to understand. 
        // do not override in most cases

        

        switch(currentState)
        {
            case EnemyStates.Idle:
                // basically, if you cahnge the currentState
                // a change is detected, that's how we tell 
                // when to trigger Enter/Exit 
                if (prevState != currentState) { EnterIdle(); prevState = currentState; } // if the prevState was just different, Enter
                Idle();
                if (prevState != currentState) ExitIdle(); // if the loop triggered the change, Exit
                break;
            case EnemyStates.Searching:
                if (prevState != currentState) { EnterSearching(); prevState = currentState; }
                Searching();
                if (prevState != currentState) ExitSearching();
                break;
            case EnemyStates.Chasing:
                if (prevState != currentState) { EnterChasing(); prevState = currentState; }
                Chasing();
                if (prevState != currentState) ExitChasing();
                break;
            case EnemyStates.Attacking:
                if (prevState != currentState) { EnterAttacking(); prevState = currentState; }
                Attacking();
                if (prevState != currentState) ExitAttacking();
                break;
            case EnemyStates.Following:
                if (prevState != currentState) { EnterFollowing(); prevState = currentState; }
                Following();
                if (prevState != currentState) ExitFollowing();
                break;
            case EnemyStates.Frantic:
                if (prevState != currentState) { EnterFrantic(); prevState = currentState; }
                Frantic();
                if (prevState != currentState) ExitFrantic();
                break;
        }

        
        
    }
    // ---------- IDLE ----------------------
    public virtual void EnterIdle()
    {
        agent.isStopped = true; // stay still
    }
    public virtual void Idle()
    {
        // loop something here? 
    }
    public virtual void ExitIdle()
    {
        // do something here?
    }
    // ---------- SEARCHING ----------------------
    public virtual void EnterSearching()
    {
        // do something here?
    }
    public virtual void Searching()
    {
        // do something here?
    }
    public virtual void ExitSearching()
    {
        // do something here?
    }
    // ---------- CHASING ----------------------
    public virtual void EnterChasing()
    {
        agent.speed = baseStats.speed * baseStats.sprintSpeedMult;
    }
    public virtual void Chasing()
    {
        if (agent.enabled) agent.SetDestination(player.transform.position);
    }
    public virtual void ExitChasing()
    {
        agent.speed = baseStats.speed;
    }
    // ---------- ATTACKING ----------------------
    public virtual void EnterAttacking()
    {
        // i will not bother with ts yet. 
    }
    public virtual void Attacking()
    {
        Debug.Log(gameObject.name+" just attacked!");
    }
    public virtual void ExitAttacking() {}
    // ---------- FOLLOWING ----------------------
    public virtual void EnterFollowing()
    {
        agent.enabled = true;
        agent.speed = baseStats.speed;
        agent.isStopped = false;
    }
    public virtual void Following() {
        if (Vector3.Distance(transform.position, player.transform.position) < enemyStats.startAttackDist)
        {
            // too close, don't move. 
            agent.isStopped = true;
        } else
        {
            agent.isStopped = false;
            if (agent.enabled) agent.SetDestination(player.transform.position);
        }
    }
    public virtual void ExitFollowing()
    {
        // do something here
    }
    // ---------- FRANTIC ----------------------
    float franticPeriod = 10f;
    float timeEnteredFrantic = -999f;
    public virtual void EnterFrantic() {
        agent.isStopped = false;
        agent.speed = baseStats.speed * baseStats.sprintSpeedMult;
        timeEnteredFrantic = Time.time;
    }
    public virtual void Frantic()
    {
        if (Time.time - timeEnteredFrantic > franticPeriod)
        {
            SetState(EnemyStates.Idle);
            return;
        } else
        {
            if (!ReachedDestination()) return; // haven't reached your destination yet?

            // while you are frantically running around
            Vector3 targetPos = transform.position;

            Vector3 randOffset = Random.insideUnitSphere * 15f;
            randOffset.y = 0;

            Vector3 chosenPos = transform.position + randOffset;

            if (NavMesh.SamplePosition(chosenPos, out var hit, 3f, NavMesh.AllAreas))
            {
                targetPos = chosenPos;
            } 
            
            agent.SetDestination(targetPos);
        }
    }
    public virtual void ExitFrantic()
    {
        // do something here
    }
    public bool ReachedDestination()
    {
        if (!agent.pathPending &&
        agent.remainingDistance <= agent.stoppingDistance &&
        (!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
        {
            return true;
        } 
        return false;
    }
}

public enum EnemyStates
{
    Idle,
    Searching, 
    Chasing,
    Attacking,
    Following, 
    Frantic,
    Dead,
}