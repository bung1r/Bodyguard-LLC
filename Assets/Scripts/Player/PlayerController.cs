using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

// Enum state is really a mess, I should just remove Sneaking (will refactor later message)

public class PlayerController : MonoBehaviour
{

    [SerializeField] Transform playerCamera;

    [HideInInspector] public enum PlayerMovementState { Running, Sneaking, Airborne }
    // Running is basically an idle state as well, as action + weapon should retain sprint speed(?)
    PlayerMovementState movementState; 

    private bool inAction;
    private bool inGun;
    private bool isGrabbing;

    [SerializeField] private float jumpHeight = 0.65f;
    [SerializeField] private float gravity = -9.8f;
    private float speed;
    private EmployerAI employer;
    private StatManager statManager;
    private RuntimeStats runtimeStats;
    private RuntimeBaseStats baseStats;
    private RuntimePlayerStats playerStats;
    private CharacterController controller;
    private Vector3 moveInput; 
    private Vector3 velocity;
    private List<Checkpoint> checkpoints = new List<Checkpoint>();

    [SerializeField] private float kickForce = 9.0f;

    private Vector3 kickOffset = new Vector3(0,-0.5f,0);
    
    public GameObject guntext;

    private bool sneakLatch; //stupid fucking variable but its very late rn

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statManager = GetComponent<StatManager>();
        controller = GetComponent<CharacterController>();
        employer = FindFirstObjectByType<EmployerAI>();
        if (employer == null) Debug.LogError("Employer could not be found");

        if (statManager != null)
        {
            runtimeStats = statManager.GetRuntimeStats();
            baseStats = runtimeStats.GetBaseStats();
            playerStats = runtimeStats.GetPlayerStats();
        }

        movementState = PlayerMovementState.Running;
        speed = baseStats.speed * baseStats.sprintSpeedMult;

        inAction = false;
    }

    // MOVEMENT

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && (movementState == PlayerMovementState.Running || movementState == PlayerMovementState.Sneaking))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnShift(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            sneakLatch = true;
        }

        if (context.canceled)
        {
            sneakLatch = false;
        }
    }

    // public void OnCrouch(InputAction.CallbackContext context)
    // {
    //     if (context.performed)
    //     {
    //         baseStats.isCrouching = !baseStats.isCrouching;
    //     }
    // }

    // ACTIONS

    //  handles stuff like talking to the employer and doors
    bool holdingE = false; // fuck ass variable
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            holdingE = true;
            int layer = (1 << 9);
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 5.0f, layer)) 
            {
                // check if we got the employer 
                Debug.Log("Interacted with the employer");
                if (hit.transform.root.TryGetComponent<EmployerAI>(out var employerAI))
                {
                    if (employerAI.GetState() == EnemyStates.Following)
                    {
                        employerAI.SetState(EnemyStates.Idle);
                    } else
                    {
                        employerAI.SetState(EnemyStates.Following);
                    }
                }
            }
        }

        if (context.canceled)
        {
            holdingE = false;
        }
    }
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // a bunch of stuff, 'innit?

            // Shooting should take priority, probably.

            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit grabHit, 4.5f, (1 << 8)))
            {
                if (isGrabbing)
                {
                    inAction = false;
                    isGrabbing = false;
                    grabHit.collider.gameObject.GetComponent<BaseProp>().EndGrab();
                } else if (!inAction)
                {
                    inAction = true;
                    isGrabbing = true;
                    grabHit.collider.gameObject.GetComponent<BaseProp>().StartGrab(playerCamera.transform);   
                }
            }

            if (employer.GetState() == EnemyStates.Following)
            {
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 5.0f, (1 << 10))
                && Vector3.Distance(transform.position, hit.point) <= 10f)
                {
                    // facing the floor and point is less than 10 meters  away
                    employer.SetGoToPosPosition(hit.point);
                    employer.SetState(EnemyStates.GoToPos); // go to the point in the floor.
                } else if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit employerHit, 5.0f, (1 << 9))
                && Vector3.Distance(transform.position, employerHit.point) <= 5f)
                {
                    // if you click on the employer, he will go back to work. 
                    employer.SetState(EnemyStates.Chasing);
                }
                
            }
        }
    }
    public void OnRightClick(InputAction.CallbackContext context)
    {
        
        // set a checkpoint!
        if (context.performed)
        {
            if (!holdingE)
            {
                Debug.Log("The Player has made a checkpoint");
                checkpoints.Add(new Checkpoint());

            } else
            {
                // Debug.Log(checkpoints.Count);
                // recall to the last checkpoint
                if (checkpoints.Count > 0)
                {
                    Debug.Log("The Player has recalled to the last checkpoint");
                    Checkpoint checkpoint = checkpoints[checkpoints.Count - 1];
                    checkpoint.ReturnByDeath(); // revert everything to a previous state
                }
            }
        }
        
    }
    // Kick input/check logic
    public void OnKick(InputAction.CallbackContext context)
    {
        if (context.performed && inAction == false)
        {
            inAction = true;
            Invoke(nameof(ResetFromAction), 0.5f);
            KickDo();
        }
    }

    bool checkIsGrounded(GameObject gameObject)
    {
        float distToGround = gameObject.GetComponent<Collider>().bounds.extents.y;
        return Physics.Raycast(GetComponent<Collider>().gameObject.transform.position, -Vector3.up, distToGround + 0.2f);
    }

    // Kick hit logic
    void KickDo()
    {
        int kickLayerMask = (1 << 7) | (1 << 8); //raycast only hits enemies and phys prop layers
        if(!Physics.Raycast(transform.position + kickOffset, playerCamera.transform.forward, out RaycastHit hit, 5.0f, kickLayerMask)) {return;}
        
        Debug.Log($"hit {hit.collider.gameObject.name} with kick!");

        if (hit.collider.gameObject.layer == 7)
        {
            StatManager enemyStatManager = hit.collider.gameObject.transform.parent.transform.parent.GetComponent<StatManager>(); // AAUUGHHHGHGHHH
            DamageData damageData = new DamageData{source = gameObject, damageType = DamageType.Blunt, damageAmount = 0.5f};
            enemyStatManager.TakeDamage(damageData);

        } else {
            if (!hit.collider.gameObject.GetComponent<Rigidbody>()) {return;}

            Vector3 direction = playerCamera.transform.forward; //(hit.transform.position - transform.position).normalized;
            if(checkIsGrounded(hit.collider.gameObject))
            {
                direction.z = 0.0f;
            }
            hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(direction * kickForce, ForceMode.Impulse);   
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed && inAction == false) 
        {
            inAction = true;
            inGun = true;
            guntext.SetActive(true);
        }

        if (context.canceled && inAction == true && inGun == true)
        {
            guntext.SetActive(false);

            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 500.0f, ((1 << 7) | (1 << 8)) ))
            {
                Debug.Log($"hit {hit.collider.gameObject.name} with bullet!");

                if (hit.collider.gameObject.layer == 7)
                {
                    StatManager enemyStatManager = hit.collider.gameObject.transform.parent.transform.parent.GetComponent<StatManager>(); // AAUUGHHHGHGHHH
                    DamageData damageData = new DamageData{source = gameObject, damageType = DamageType.Pierce, damageAmount = 2f};
                    enemyStatManager.TakeDamage(damageData);

                } else {
                    if (!hit.collider.gameObject.GetComponent<Rigidbody>()) {return;}

                    Vector3 direction = playerCamera.transform.forward; //(hit.transform.position - transform.position).normalized;
                    hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(direction * 7.0f, ForceMode.Impulse);
                    hit.collider.gameObject.GetComponent<BaseProp>().TakeDamage(new DamageData{source = gameObject, damageType = DamageType.Bullet, damageAmount = 2f});
                }
            }

            inGun = false;

            Invoke(nameof(ResetFromAction), 1f);
        }
    }


    void ResetFromAction()
    {
        inAction = false;
        //anything else here(?)
    } 

    // State Logic

    void Update()
    {

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        StateCheck();
    }

    void StateCheck()
    {
        if (!controller.isGrounded)
        {
            movementState = PlayerMovementState.Airborne;
        } else {
            if (sneakLatch)
            {
                movementState = PlayerMovementState.Sneaking;
                speed = baseStats.speed;
            } else
            {
                movementState = PlayerMovementState.Running;
                speed = baseStats.speed * baseStats.sprintSpeedMult;
            }
        }
    }
    // ----------- SETTERS -----------------
    public void SetPlayerMovementState(PlayerMovementState state) => movementState = state;
    public void SetInAction(bool value) => inAction = value;
    public void SetIsGrabbing(bool value) => isGrabbing = value;
    // ----------- GETTERS -----------------
    public PlayerMovementState GetPlayerMovementState() => movementState;
    public bool GetInAction() => inAction;
    public bool GetIsGrabbing() => isGrabbing;
}
