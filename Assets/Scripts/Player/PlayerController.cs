using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Transform playerCamera;

    enum PlayerMovementState { Running, Sneaking, Airborne, Action, Weapon }
    // Running is basically an idle state as well, as action + weapon should retain sprint speed(?)
    PlayerMovementState movementState; 

    private bool inAction;

    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -9.8f;
    private float speed;
    private StatManager statManager;
    private RuntimeStats runtimeStats;
    private RuntimeBaseStats baseStats;
    private RuntimePlayerStats playerStats;
    private CharacterController controller;
    private Vector3 moveInput; 
    private Vector3 velocity;

    private bool sneakLatch; //stupid fucking variable but its very late rn

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statManager = GetComponent<StatManager>();
        controller = GetComponent<CharacterController>();

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
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
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
    }

    // Kick input/check logic
    public void OnKick(InputAction.CallbackContext context)
    {
        if (context.performed && inAction == false)
        {
            inAction = true;
            KickDo();
            Invoke(nameof(ResetFromAction), 0.5f);
        }
    }

    // Kick hit logic
    void KickDo()
    {
        int kickLayerMask = (1 << 7) | (1 << 8); //raycast only hits enemies and prop layers
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 5.0f, ~kickLayerMask))
        {
            Debug.Log("hit something with kick!");
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
}
