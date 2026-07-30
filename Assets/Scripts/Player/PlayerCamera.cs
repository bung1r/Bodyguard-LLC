using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{

    [SerializeField] float sensX;
    [SerializeField] float sensY;

    [SerializeField] private InputActionAsset playerControls;
    private Vector2 lookInput;
    private InputAction lookAction;
    

    [SerializeField] Transform playerCamera;
    float xRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lookAction = playerControls.FindActionMap("Player").FindAction("Look");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lookAction.performed += context => lookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => lookInput = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = lookInput.x * sensX * Time.deltaTime;
        float mouseY = lookInput.y * sensY * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
