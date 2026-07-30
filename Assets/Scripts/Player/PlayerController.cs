using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;

    private StatManager statManager;
    private RuntimeStats runtimeStats;
    private RuntimeBaseStats baseStats;
    private RuntimePlayerStats playerStats;
    private CharacterController controller;
    private Vector3 moveInput;
    private Vector3 velocity;

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

            baseStats.isSprinting = true;
            baseStats.isWalking = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // e to interact
    //  handles stuff like talking to the employer and doors
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
        }
    }

    public void OnShift(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            baseStats.isWalking = true;
            baseStats.isSprinting = false;
        }

        if (context.canceled)
        {
            baseStats.isWalking = false;
            baseStats.isSprinting = true;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            baseStats.isCrouching = !baseStats.isCrouching;
        }
    }
    // calc stands for calculate
    float CalcSpeed() {
        float speed = baseStats.speed;
        if (baseStats.isSprinting)
        {
            speed *= baseStats.sprintSpeedMult;
        }

        if (baseStats.isCrouching)
        {
            speed *= 0.3f;
        }

        return speed;
    }
    // Update is called once per frame
    void Update()
    {
        speed = CalcSpeed();
        Debug.Log(speed);

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
