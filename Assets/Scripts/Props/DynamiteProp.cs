using UnityEngine;

public class DynamiteProp : BaseProp
{

    public override void OnActivation()
    {
        
    }

    public override void TakeDamage(DamageData damageData)
    {
        if (damageData.damageType == DamageType.Bullet)
        {
            // KAPOW!!!
        }
        base.TakeDamage(damageData);
    }
}
