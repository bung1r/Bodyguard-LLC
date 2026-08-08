using UnityEngine;

public class LampProp : BaseProp
{
    private GameObject rope;
    private GameObject container;
    [SerializeField] private Collider wireCollider;

    private bool isDetached;

    protected override void Start()
    {
        base.Start();
        container = transform.parent.gameObject;
        rope = transform.GetChild(0).gameObject;
        isDetached = false;
    }

    public override void OnActivation()
    {
        
    }

    public override void TakeDamage(DamageData damageData)
    {
        if (isDetached == false)
        {
            FixedJoint toRopeJoint = GetComponent<FixedJoint>();
            Destroy(wireCollider);
            Destroy(toRopeJoint); 
            rope.SetActive(false);
            isDetached = true;
        }
    }


}