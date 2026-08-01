using System.Collections.Generic;
using System.Linq;
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
    public GameObject bulletPrefab;
    public EnemyStates initialState = EnemyStates.Wandering;
    [SerializeField] protected EnemyStates currentState; // ONLY change this. Use SetState()
    protected Vector3 goToPosPosition;
    protected bool enableVisionCone = true;

    public int seed = 1_000_000;
    System.Random wanderRNG;
    
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
        wanderRNG = new System.Random(seed + 1);
        SetState(initialState);
    }
    protected virtual void Update()
    {
        Think();
    }
    float viewAngle = 110f;
    protected Transform[] visiblePlayers;
    protected Transform[] visibleTraps;
    protected Transform[] visibleEmployers;
    protected Vector3 lastPlayerPos;
    protected virtual void VisionCone()
    {
        Transform[] GetTransformsWithinCone(Collider[] colliders)
        {
            HashSet<Transform> transformsHash = new HashSet<Transform>(); 
            foreach (Collider collider in colliders)
            {
                Vector3 direction = (collider.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, direction);

                if (angle > viewAngle / 2f) continue;
                transformsHash.Add(collider.transform.root);
            }

            return transformsHash.ToArray(); // fallback
        }
        
        int employerMask = (1 << 9);
        int playerMask = (1 << 6);
        int trapMask = (1 << 11);
        
        Collider[] employerCollider = Physics.OverlapSphere(transform.position, enemyStats.aggroDistance, employerMask);
        visibleEmployers = GetTransformsWithinCone(employerCollider);
        Collider[] playerCollider = Physics.OverlapSphere(transform.position, enemyStats.aggroDistance, playerMask);
        visiblePlayers = GetTransformsWithinCone(playerCollider);
        if (visiblePlayers.Length > 0)
        {
            lastPlayerPos = visiblePlayers[0].transform.position;
        }
        Collider[] trapColliders = Physics.OverlapSphere(transform.position, enemyStats.aggroDistance, trapMask);
        visibleTraps = GetTransformsWithinCone(trapColliders);
    }
    
    public EnemyStates GetState() => currentState;
    public virtual void SetState(EnemyStates enemyState)
    {

        if (enemyState != currentState)
        {
            // exit the previous state
            switch(currentState)
            {
                case EnemyStates.Idle: ExitIdle(); break;
                case EnemyStates.Wandering: ExitWandering(); break;
                case EnemyStates.Searching: ExitSearching(); break;
                case EnemyStates.Attacking: ExitAttacking(); break;
                case EnemyStates.Chasing: ExitChasing(); break;
                case EnemyStates.Following: ExitFollowing(); break;
                case EnemyStates.Frantic: ExitFrantic(); break;
                case EnemyStates.GoToPos: ExitGoToPos(); break;
            }

            currentState = enemyState;

            // enter the next state
            switch(currentState)
            {
                case EnemyStates.Idle: EnterIdle(); break;
                case EnemyStates.Wandering: EnterWandering(); break;
                case EnemyStates.Searching: EnterSearching(); break;
                case EnemyStates.Attacking: EnterAttacking(); break;
                case EnemyStates.Chasing: EnterChasing(); break;
                case EnemyStates.Following: EnterFollowing(); break;
                case EnemyStates.Frantic: EnterFrantic(); break;
                case EnemyStates.GoToPos: EnterGoToPos(); break;
            }
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
        if (enableVisionCone) VisionCone();

        switch(currentState)
        {
            case EnemyStates.Idle: Idle(); break;
            case EnemyStates.Wandering: Wandering(); break;
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
        agent.isStopped = false;
    }
    // ---------- WANDERING ----------------------
    public virtual void EnterWandering()
    {
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.speed = baseStats.speed;
    }
    public virtual void Wandering()
    {

        if (visiblePlayers.Length > 0)
        {
            SetState(EnemyStates.Chasing);
            return;
        }

        // Add code here for 'hearing gunshots' or stuff
        
        if (ReachedDestination())
        {
            Collider[] nodes = Physics.OverlapSphere(transform.position, 15f, 1 << 12);
            if (nodes.Length > 0)
            {
                Collider selectedNode = nodes[wanderRNG.Next(0, nodes.Length)];
                
                agent.SetDestination(selectedNode.transform.position);
            } else
            {
                Debug.Log("What? No nodes?");
            } 
        }
    }
    public virtual void ExitWandering()
    {
        
    }
    // ---------- SEARCHING ----------------------
    float searchPeriod = 10f;
    float startSearch = -999f;
    public virtual void EnterSearching()
    {
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.speed = baseStats.speed * baseStats.sprintSpeedMult;
        if (lastPlayerPos == null) lastPlayerPos = transform.position;
        agent.SetDestination(lastPlayerPos);
        startSearch = Time.time;
    }
    public virtual void Searching()
    {
        if (Time.time - startSearch > searchPeriod)
        {
            SetState(EnemyStates.Wandering);
            return;
        }

        if (ReachedDestination())
        {
            Collider[] nodes = Physics.OverlapSphere(transform.position, 15f, 1 << 12);
            if (nodes.Length > 0)
            {
                Collider selectedNode = nodes[wanderRNG.Next(0, nodes.Length)];
                agent.SetDestination(selectedNode.transform.position);
            } else
            {
                Debug.Log("What? No nodes?");
            } 
        }
        
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
        
        if (Vector3.Distance(transform.position, player.transform.position) > enemyStats.deaggroDistance
        && visiblePlayers.Length == 0)
        {
            SetState(EnemyStates.Searching);
            return;
        } else if (Vector3.Distance(transform.position, player.transform.position) <= enemyStats.startAttackDist)
        {
            SetState(EnemyStates.Attacking);
            return;
        }

        agent.SetDestination(player.transform.position);
    }
    public virtual void ExitChasing()
    {
        agent.speed = baseStats.speed;
    }
    // ---------- ATTACKING ----------------------
    protected float lastAttacked = -999f;
    public virtual void EnterAttacking()
    {
        // i will not bother with ts yet. 
        agent.isStopped = true;
        lastAttacked = Time.time;
    }
    public virtual void Attacking()
    {
        // Add code for attacking eventually lol.
        if (Vector3.Distance(transform.position, player.transform.position) > enemyStats.startAttackDist + 0.5f)
        {
            SetState(EnemyStates.Chasing);
            return;
        }

        if (Time.time - lastAttacked > enemyStats.timeBetweenAttacks)
        {
            Attack();
            lastAttacked = Time.time;
        }
        
        RotateManually(player.transform.position);
    }
    public virtual void ExitAttacking()
    {
        agent.updateRotation = true;
        agent.isStopped = false;
    }
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
        !agent.hasPath)
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
    public void VisualizeBullet(Vector3 pos1, Vector3 pos2)
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = (pos1 + pos2) / 2f;
        bullet.transform.localScale = new Vector3(
            bullet.transform.localScale.x, 
            bullet.transform.localScale.y, 
            Vector3.Distance(pos1, pos2)
        );
        bullet.transform.localRotation = Quaternion.LookRotation(pos1 - pos2);
        
        Destroy(bullet, 0.5f);
    }
    public virtual void Attack() // whatever your attack is 
    {
        Attack(new DamageData{source = gameObject, damageType = DamageType.Pierce, damageAmount = 1f}, 5f);
    }
    public virtual void Attack(DamageData damageData, float spreadAngle) // whatever your attack is 
    {
        Vector3 target = transform.position;
        if (visibleEmployers.Length > 0)
        {
            target = visibleEmployers[0].transform.position;
        } else if (visiblePlayers.Length > 0)
        {
            target = visiblePlayers[0].transform.position;
        }
        Vector3 direction = (target - transform.position).normalized;
        int attackMask = (1 << 3) | (1 << 6) | (1 << 9) | (1 << 10); // obstacles, players, floor
        float yaw = Random.Range(-spreadAngle, spreadAngle);
        float pitch = Random.Range(-spreadAngle, spreadAngle);
        Quaternion spread = Quaternion.Euler(pitch, yaw, 0);

        Vector3 finalDirection = spread * direction;
        
        if(Physics.Raycast(transform.position, finalDirection, out RaycastHit hit, 200f, attackMask))
        {
            VisualizeBullet(transform.position, hit.point);
            if (hit.transform.root.TryGetComponent<StatManager>(out var statManager))
            {
                statManager.TakeDamage(damageData);
            }

        }
    }
    public void EnemyDeath()
    {
        //stop ai logic
        enabled = false;

        //stop movement
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        //disable collision
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        //play blood spatter or death animation here

        Destroy(gameObject, 0.0f);
    }
    


}

public enum EnemyStates
{
    Idle,
    Wandering,
    Searching, 
    Chasing,
    Attacking,
    Following, 
    Frantic,
    GoToPos,
    Dead,
}