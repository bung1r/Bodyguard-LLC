using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    // where the player will interact
    // (mouse clicks, e to interact, stuff like that)
    private PlayerControls inputActions;


    
    void Awake()
    {
        inputActions = new PlayerControls();
        
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        
    }

}