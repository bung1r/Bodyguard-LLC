using UnityEngine;
using System.Collections;

public class DynamiteProp : BaseProp
{

    [SerializeField] private float explosionRadius;
    private bool isExploding;

    public override void OnActivation()
    {
        isExploding = true;
        StartCoroutine(Explosion(0.1f));
    }

    public override void TakeDamage(DamageData damageData)
    {
        if (damageData.damageType == DamageType.Bullet || damageData.damageType == DamageType.Explosion)
        {
            // KAPOW!!!
            isExploding = true;
            StartCoroutine(Explosion(0.1f));
        }
        if (damageData.damageType == DamageType.Fire)
        {
            // KAPOW!!!
            isExploding = true;
            StartCoroutine(Explosion(3f));
        }
        base.TakeDamage(damageData);
    }

    private IEnumerator Explosion(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("Exploding!");

        Collider[] array = Physics.OverlapSphere(transform.position, explosionRadius, (1 << 6) | (1 << 7) | (1 << 8) | (1 << 9), QueryTriggerInteraction.Collide);
            
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].gameObject.layer == 7)
            {
                Debug.Log($"Hit enemy: {array[i].gameObject}");
                StatManager enemyStatManager = array[i].gameObject.transform.parent.transform.parent.GetComponent<StatManager>(); // AAUUGHHHGHGHHH
                DamageData explosionDamageData = new DamageData{source = gameObject, damageType = DamageType.Explosion, damageAmount = 1.5f};
                enemyStatManager.TakeDamage(explosionDamageData);
            }
            if (array[i].gameObject.layer == 8 && array[i].gameObject != gameObject)
            {
                Debug.Log($"Hit prop: {array[i].gameObject}");
                Rigidbody rb = array[i].GetComponent<Rigidbody>();
                rb.AddExplosionForce(50.0f, transform.position, explosionRadius, 0.5f, ForceMode.Acceleration);
                array[i].gameObject.GetComponent<BaseProp>().TakeDamage(new DamageData{source = gameObject, damageType = DamageType.Explosion, damageAmount = 2.0f});
            }
            // if (array[i].gameObject.layer == 9)
            // {
            //     Debug.Log($"Hit employer! You scallywag!");
            //     StatManager enemyStatManager = array[i].GetComponent<StatManager>(); // AAUUGHHHGHGHHH
            //     DamageData explosionDamageData = new DamageData{source = gameObject, damageType = DamageType.Explosion, damageAmount = 1.5f};
            //     enemyStatManager.TakeDamage(explosionDamageData);
            // }
        }

        isExploding = false;

        DestroyProp();

    }

    public override void DestroyProp()
    {
        if (!isExploding)
        {
            base.DestroyProp();
        }
        
    }
}
