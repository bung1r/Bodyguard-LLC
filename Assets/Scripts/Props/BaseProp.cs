using UnityEngine;

public class BaseProp : MonoBehaviour
{
    private Rigidbody propBody;
    public Prop propProperties;

    private float currentDurability;

    // All props are grabbable
    protected virtual void Start()
    {
        propBody = GetComponent<Rigidbody>();
        currentDurability = propProperties.durability;
    }

    // Update is called once per frame
    protected virtual void Update()
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
            TakeDamage(1.0f);
        }
    }

    public virtual void OnActivation()
    {
        
    }

    public void TakeDamage(float damageAmount)
    {
        currentDurability -= damageAmount;
        if (currentDurability <= 0)
        {
            DestroyProp();
        }
    }

    public virtual void DestroyProp()
    {
        Destroy(gameObject, 0.5f);
    }
}
