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
    public GameObject prefab;
    public GameObject bulletPrefab;
    public Transform barrel;
    public EnemyStates initialState = EnemyStates.Wandering;
    public EnemyStates afterStunState = EnemyStates.Searching;
    [SerializeField] protected EnemyStates currentState; // ONLY change this. Use SetState()
    protected Vector3 goToPosPosition;
    protected HashSet<Transform> hitHash = new HashSet<Transform>();
    protected bool enableVisionCone = true;
    protected float stunTime = 1f;
    [SerializeField] protected bool enableAnimator;
    protected Animator animator;
    protected int runningHash;
    protected int walkingHash;
    protected int attackHash;
    protected int standingHash;
    protected int changeStatesHash;
    protected int changeMovementHash;
    public int seed = 1_000_000;
    GameRandom wanderRNG;
    void Awake()
    {
        wanderRNG = new GameRandom(seed + 1);

        animator = GetComponent<Animator>();
        if (enableAnimator)
        {
            runningHash = Animator.StringToHash("Running");
            walkingHash = Animator.StringToHash("Walking");
            standingHash = Animator.StringToHash("Standing");
            attackHash = Animator.StringToHash("Attack");
            changeStatesHash = Animator.StringToHash("ChangeStates");
            changeMovementHash = Animator.StringToHash("ChangeMovement");
        }

    }
    
    public virtual void Start()
    {
        // enemy stuff
        statManager = GetComponent<StatManager>();
        agent = GetComponent<NavMeshAgent>();


        

        runtimeStats = statManager.GetRuntimeStats();
        baseStats = runtimeStats.GetBaseStats();
        enemyStats = runtimeStats.GetEnemyStats();

        // player stuff
        player = FindFirstObjectByType<PlayerController>().transform;
        // Debug.Log(player);

        // initializing enemy variables
        agent.speed = baseStats.speed;
        
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

            HandleAnimStates(true, false);
        }
    }
    // When in the GoToPos state, this value determines where it will go. 
    public void SetStateDirect(EnemyStates enemyState) // NICHE!! Will not activate enter or exit methods. 
    {
        currentState = enemyState;
    }
    
    public void SetGoToPosPosition(Vector3 position)
    {
        goToPosPosition = position;
    }
    public virtual void HandleAnimStates(bool changeState = false, bool attack = false)
    {
        if (!enableAnimator) return;

        bool lastWalking = animator.GetBool(walkingHash);
        bool lastRunning = animator.GetBool(runningHash);
        bool lastStanding = animator.GetBool(standingHash);

        animator.SetBool(walkingHash, false);
        animator.SetBool(runningHash, false);
        animator.SetBool(standingHash, false);

        if (!agent.isStopped && agent.velocity.sqrMagnitude > 0.01f)
        {
            if (agent.speed > baseStats.speed)
            {
                animator.SetBool(runningHash, true);
            } else
            {
                animator.SetBool(walkingHash, true);
            }
        } else
        {
            animator.SetBool(standingHash, true);
        }

        if (lastWalking != animator.GetBool(walkingHash) ||
            lastRunning != animator.GetBool(runningHash) || 
            lastStanding != animator.GetBool(standingHash))
        {
            animator.SetTrigger(changeMovementHash);
        }

        if (attack) animator.SetTrigger(attackHash);
        if (changeState) animator.SetTrigger(changeStatesHash);
    }
    public virtual void Think()
    {
        // The AI is thinking, pretty easy to understand. 
        // do not override in most cases
        if (enableVisionCone) VisionCone();
        if (enableAnimator) HandleAnimStates();

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

        if (visiblePlayers.Length > 0 || visibleEmployers.Length > 0)
        {
            SetState(EnemyStates.Chasing);
            return;
        }

        // Add code here for 'hearing gunshots' or stuff
        
        if (ReachedDestination(1f))
        {
            Collider[] nodes = Physics.OverlapSphere(transform.position, 15f, 1 << 12);
            // Debug.Log(nodes.Length);
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

        if (visiblePlayers.Length > 0)
        {
            SetState(EnemyStates.Chasing);
            return;
        }

        if (ReachedDestination(1f))
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
        // prioritze the employer
        Vector3 target;
        if (visibleEmployers.Length > 0)
        {
            target = visibleEmployers[0].position;
        } else
        {
            target = player.transform.position;
        }

        if (Vector3.Distance(transform.position, target) > enemyStats.deaggroDistance
        && visiblePlayers.Length == 0)
        {
            SetState(EnemyStates.Searching);
            return;
        } else if (Vector3.Distance(transform.position, target) <= enemyStats.startAttackDist)
        {
            SetState(EnemyStates.Attacking);
            return;
        }

        agent.SetDestination(target);
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
        Vector3 target;
        if (visibleEmployers.Length > 0)
        {
            target = visibleEmployers[0].position;
        } else
        {
            target = player.transform.position;
        }

        if (Vector3.Distance(transform.position, target) > enemyStats.startAttackDist + 0.5f)
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
    protected float lastStunned = -999f;
    public virtual void EnterStunned()
    {
        agent.isStopped = true;
        agent.updateRotation = false;
        agent.ResetPath();
        lastStunned = Time.time;
    }
    public virtual void Stunned()
    {
        if (Time.time - lastStunned > stunTime)
        {
            SetState(afterStunState);
            return;
        }
    }
    public virtual void ExitStunned()
    {
        agent.isStopped = false;
        agent.updateRotation = true;
    }
    // -------- Helper Methods! Just help for all EnemyAIs, basically -----
    public bool ReachedDestination()
    {
        return ReachedDestination(0f);
    }

    public bool ReachedDestination(float margin)
    {
        return !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance + margin &&
            (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
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
        if (barrel == null) return;
        Vector3 target = transform.position;
        if (visibleEmployers.Length > 0)
        {
            target = visibleEmployers[0].transform.position;
        } else if (visiblePlayers.Length > 0)
        {
            target = visiblePlayers[0].transform.position;
        }
        Vector3 direction = (target - barrel.position).normalized;
        int attackMask = (1 << 3) | (1 << 6) | (1 << 9) | (1 << 10); // obstacles, players, floor
        float yaw = Random.Range(-spreadAngle, spreadAngle);
        float pitch = Random.Range(-spreadAngle, spreadAngle);
        Quaternion spread = Quaternion.Euler(pitch, yaw, 0);

        Vector3 finalDirection = spread * direction;
        
    
        HandleAnimStates(true, true);

        if(Physics.Raycast(barrel.position, finalDirection, out RaycastHit hit, 200f, attackMask))
        {
            VisualizeBullet(barrel.position, hit.point);
            if (hit.transform.root.TryGetComponent<StatManager>(out var statManager))
            {
                statManager.TakeDamage(damageData);
            }

        }
    }
    public virtual void EnemyDeath()
    {
        //stop ai logic
        enabled = false;

        // stop movement
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
    public void ReceiveSound(Vector3 position, float decibels)
    {
        if (visiblePlayers == null || visibleEmployers == null) return;
        // Debug.Log(transform.name + " hears the sound of " + decibels.ToString());
        lastPlayerPos = position;

        if (visiblePlayers.Length == 0 && 
        visibleEmployers.Length == 0 && 
        currentState != EnemyStates.Attacking && 
        currentState != EnemyStates.Chasing && 
        currentState != EnemyStates.Searching)
        {
            // Debug.Log(transform.name + " hears the sound 2!");
            SetState(EnemyStates.Searching);
            return;
        } 
    }

    void OnDestroy()
    {
        RoundManager.Instance.enemies.Remove(this);
    }
    
    // ------- SETTERS --------------
    public void SetWanderRNG(int nextCalls)
    {
        wanderRNG = new GameRandom(seed + 1);
        for (int i = 0; i < nextCalls; i++)
        {
            wanderRNG.Next();
        }
    } 
    public void SetLastAttacked(float value) => lastAttacked = value;
    public void SetLastStunned(float value) => lastStunned = value;
    public void SetLastPlayerPos(Vector3 value) => lastPlayerPos = value;
    public void SetIsRunningAnim(bool value) => animator.SetBool(runningHash, value);
    public void SetIsWalkingAnim(bool value) => animator.SetBool(walkingHash, value);
    public void SetIsStandingAnim(bool value) => animator.SetBool(standingHash, value);
    // ------- GETTERS --------------
    public float GetLastAttacked() => lastAttacked;
    public float GetLastStunned() => lastStunned;
    public EnemyStates GetCurrentState() => currentState;
    public RuntimeStats GetRuntimeStats() => runtimeStats;
    public int GetWanderRNGCalls() => wanderRNG.CallsMade;
    public Vector3 GetLastPlayerPos() => lastPlayerPos;
    public bool GetEnableAnimator() => enableAnimator;
    public bool GetIsRunningAnim() => animator.GetBool(runningHash);
    public bool GetIsWalkingAnim() => animator.GetBool(walkingHash);
    public bool GetIsStandingAnim() => animator.GetBool(standingHash);
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
    Stunned,
    Dead,
}