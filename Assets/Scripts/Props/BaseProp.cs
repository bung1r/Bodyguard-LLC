using UnityEngine;

public class BaseProp : MonoBehaviour
{

    private Rigidbody propBody;
    public Prop propProperties;

    // All props are grabbable
    void Start()
    {
        propBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            if (propBody.linearVelocity.magnitude < propProperties.minDamageVelocity) {return;}

            StatManager enemyStatManager = collision.transform.parent.transform.parent.GetComponent<StatManager>();
            float damageAmount = (propBody.linearVelocity.magnitude / 20) * propProperties.damageVelocityMultiplier;
            DamageData damageData = new DamageData{source = gameObject, damageType = propProperties.damageType, damageAmount = damageAmount};
            enemyStatManager.TakeDamage(damageData);
        }
    }
}
