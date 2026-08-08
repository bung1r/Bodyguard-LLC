using UnityEngine;

public class TripwireProp : BaseProp
{
    [SerializeField] private OutputSocket outputSocket;
    private float tripTimer;

    protected override void Start()
    {
        base.Start();
        tripTimer = 0.0f;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        tripTimer += Time.deltaTime;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (base.GetIsGrabbed()) {return;}

        if ((collision.gameObject.layer == 7 || collision.gameObject.layer == 8 || collision.gameObject.layer == 9) && tripTimer >= 1.0f)
        {
            tripTimer = 0.0f;
            outputSocket.ActivatePlug();
            Debug.Log("I'm triggering!!!!");
        }

        base.OnCollisionEnter(collision);
    }


}