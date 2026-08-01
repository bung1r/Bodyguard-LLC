using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerCheckpoint : ICheckpointBase
{
    GameObject playerRef;
    Vector3 position;
    Quaternion rotation;
    PlayerController.PlayerMovementState movementState;
    float currentHP;
    // put more stats like ammo and holding and I DON'T KNOW OK???
    
    public void ReturnByDeath(float timeSaved)
    {
        if (playerRef == null) return;

        playerRef.GetComponent<CharacterController>().enabled = false;
        playerRef.transform.position = position;
        playerRef.transform.rotation = rotation;
        playerRef.GetComponent<CharacterController>().enabled = true;
        playerRef.GetComponent<PlayerController>().SetPlayerMovementState(movementState);
        playerRef.GetComponent<StatManager>().GetRuntimeStats().GetBaseStats().currentHealth = currentHP;
    }
    public PlayerCheckpoint(PlayerController player)
    {
        position = player.transform.position;
        rotation = player.transform.rotation;
        movementState = player.GetPlayerMovementState();
        playerRef = player.gameObject;

        // this is very fun!
        currentHP = player.GetComponent<StatManager>().GetRuntimeStats().GetBaseStats().currentHealth;
        

    }
}