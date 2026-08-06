using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;
using UnityEngine.AI;

public class PropCheckpoint : ICheckpointBase
{
    GameObject propRef;
    GameObject propPrefab;
    Vector3 position;
    Quaternion rotation;
    Vector3 linearVel;
    Vector3 angularVel;
    float durability; 
    bool isGrabbed;
    public void ReturnByDeath(float timeSaved)
    {
        if (propRef == null)
        {
            propRef = RoundManager.Instantiate(propPrefab);
            RoundManager.Instance.props.Add(propRef.GetComponent<BaseProp>());
        }
        Rigidbody rb = propRef.GetComponent<Rigidbody>();
        NavMeshObstacle obstacle = propRef.GetComponent<NavMeshObstacle>();


        
        obstacle.enabled = false;
        // rb.Move(position, rotation);
        // Debug.Log(position);
        propRef.transform.position = position;
        // Debug.Log(propRef.transform.position);
        propRef.transform.rotation = rotation;
        obstacle.enabled = true;
        
        rb.linearVelocity = linearVel;
        rb.angularVelocity = angularVel;
        
        BaseProp baseProp = propRef.GetComponent<BaseProp>();
        baseProp.SetDurability(durability);
        baseProp.SetIsGrabbed(isGrabbed);
    }
    
    public PropCheckpoint(BaseProp baseProp) {
        propRef = baseProp.gameObject;
        propPrefab = baseProp.propProperties.prefab;
        position = propRef.transform.position;
        rotation = propRef.transform.rotation;
        Rigidbody rb = propRef.GetComponent<Rigidbody>();
        linearVel = rb.linearVelocity;
        angularVel = rb.angularVelocity;
        durability = baseProp.GetDurability();
        isGrabbed = baseProp.GetIsGrabbed();
    }

}