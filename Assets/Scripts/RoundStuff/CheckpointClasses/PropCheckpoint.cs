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
    bool useGravity;
    RigidbodyConstraints constraints;
    public void ReturnByDeath(float timeSaved)
    {
        if (propRef == null)
        {
            propRef = RoundManager.Instantiate(propPrefab);
            RoundManager.Instance.props.Add(propRef.GetComponent<BaseProp>());
        }


        

        NavMeshObstacle obstacle = propRef.GetComponent<NavMeshObstacle>();


        
        obstacle.enabled = false;
        // rb.Move(position, rotation);
        // Debug.Log(position);
        propRef.transform.position = position;
        // Debug.Log(propRef.transform.position);
        propRef.transform.rotation = rotation;
        obstacle.enabled = true;

        Rigidbody rb = propRef.GetComponent<Rigidbody>();
        rb.useGravity = useGravity;
        rb.linearVelocity = linearVel;
        rb.angularVelocity = angularVel;
        rb.constraints = constraints;
        
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
        useGravity = rb.useGravity;
        constraints = rb.constraints;
        durability = baseProp.GetDurability();
        isGrabbed = baseProp.GetIsGrabbed();
    }

}