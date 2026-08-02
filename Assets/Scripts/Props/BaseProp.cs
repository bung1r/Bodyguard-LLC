using UnityEngine;

public class BaseProp : MonoBehaviour
{
    public GameObject prefab;
    private Rigidbody propBody;
    public Prop propProperties;
    private Transform cameraTransform = null;

    private float currentDurability;
    private bool isGrabbed;

    private float hitTimer;

    // All props are grabbable
    protected virtual void Start()
    {
        propBody = GetComponent<Rigidbody>();
        currentDurability = propProperties.durability;
        hitTimer = 0.0f;
    }

    // Update is called once per frame
    protected virtual void LateUpdate()
    {
        if (isGrabbed == true)
        {
            Vector3 targetPos = cameraTransform.position + (cameraTransform.rotation * propProperties.grabOffset);
            transform.position = targetPos;
            transform.rotation = cameraTransform.rotation;
        }
        hitTimer += Time.deltaTime;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (isGrabbed) {return;}
        if (collision.gameObject.layer == 7)
        {
            if (hitTimer <= 0.5f) {return;}
            if (propBody.linearVelocity.magnitude < propProperties.minDamageVelocity) {return;}

            hitTimer = 0.0f;

            StatManager enemyStatManager = collision.transform.parent.transform.parent.GetComponent<StatManager>();
            float damageAmount = (propBody.linearVelocity.magnitude / 20) * propProperties.damageVelocityMultiplier;
            DamageData damageData = new DamageData{source = gameObject, damageType = propProperties.damageType, damageAmount = damageAmount};
            enemyStatManager.TakeDamage(damageData);
            TakeDamage(new DamageData{source = gameObject, damageType = DamageType.Blunt, damageAmount = 1.0f});
        }
    }

    public virtual void OnActivation()
    {
        
    }

    public virtual void StartGrab(Transform cameraTransform)
    {
        isGrabbed = true;
        if (this.cameraTransform == null) {this.cameraTransform = cameraTransform;}
    }

    public virtual void EndGrab()
    {
        isGrabbed = false;
    }


    public virtual void TakeDamage(DamageData damageData)
    {
        currentDurability -= damageData.damageAmount;
        if (currentDurability <= 0)
        {
            DestroyProp();
        }
        print(currentDurability);
    }

    public virtual void DestroyProp()
    {
        Destroy(gameObject, 0.5f);
    }

    // ---- GETTERS --------
    public float GetDurability() => currentDurability;
    public bool GetIsGrabbed() => isGrabbed;
    // ---- SETTERS --------
    public void SetDurability(float value) => currentDurability = value;
    public void SetIsGrabbed(bool value) => isGrabbed = value;

}
