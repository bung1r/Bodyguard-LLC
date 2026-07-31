using Unity.VisualScripting;
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
    [SerializeField] protected EnemyStates currentState; // ONLY change this. Preferably, use SetState()
    protected EnemyStates prevState; // do NOT change this manually
    protected Vector3 goToPosPosition;
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

        if (prevState != currentState)
        {
            // exit the previous state
            switch(prevState)
            {
                case EnemyStates.Idle: ExitIdle(); break;
                case EnemyStates.Searching: ExitSearching(); break;
                case EnemyStates.Attacking: ExitAttacking(); break;
                case EnemyStates.Chasing: ExitChasing(); break;
                case EnemyStates.Following: ExitFollowing(); break;
                case EnemyStates.Frantic: ExitFrantic(); break;
                case EnemyStates.GoToPos: ExitGoToPos(); break;
            }

            // enter the next state
            switch(currentState)
            {
                case EnemyStates.Idle: EnterIdle(); break;
                case EnemyStates.Searching: EnterSearching(); break;
                case EnemyStates.Attacking: EnterAttacking(); break;
                case EnemyStates.Chasing: EnterChasing(); break;
                case EnemyStates.Following: EnterFollowing(); break;
                case EnemyStates.Frantic: EnterFrantic(); break;
                case EnemyStates.GoToPos: EnterGoToPos(); break;
            }

            prevState = currentState;
        }
    }
    // When in the GoToPos state, this value determines where it will go. 
    public void SetGoToPosPosition(Vector3 position)
    {
        goToPosPosition = position;
    }
    
    public virtual void Think()
    {
        // The AI is thinking, pretty easy to understand. 
        // do not override in most cases

        switch(currentState)
        {
            case EnemyStates.Idle: Idle(); break;
            case EnemyStates.Searching: Searching(); break;
            case EnemyStates.Attacking: Attacking(); break;
            case EnemyStates.Chasing: Chasing(); break;
            case EnemyStates.Following: Following(); break;
            case EnemyStates.Frantic: Frantic(); break;
            case EnemyStates.GoToPos: GoToPos(); break;
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
        agent.updateRotation = true;
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
    // ---------- GOTOPOS ----------------------
    public virtual void EnterGoToPos()
    {
        agent.isStopped = false;
        agent.speed = baseStats.speed; 
        agent.SetDestination(goToPosPosition);
    }
    public virtual void GoToPos()
    {
        // do something here
        if (ReachedDestination())
        {
            SetState(EnemyStates.Idle);
        }
    }
    public virtual void ExitGoToPos()
    {
        // do something here
    }
    // -------- Helper Methods! Just help for all EnemyAIs, basically -----
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
    public void RotateManually(Vector3 targetPos) // sometimes the NavMeshAgent sucks at rotating, use this instead.
    {
        agent.updateRotation = false;
        Vector3 direction = targetPos - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float rotationSpeed = enemyStats.rotationSpeed / 10f;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
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
    GoToPos,
    Dead,
}