using UnityEngine;

public class OutputSocket : MonoBehaviour
{

    private bool isBeingPlugged;
    private GameObject? recieverPlug;

    // All props are grabbable
    void Start()
    {
        isBeingPlugged = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPlug()
    {
        if (isBeingPlugged == true) {return;} //Do we even need this? It's 2am help help
        recieverPlug = null;
        isBeingPlugged = true;
        // Visual code here, arrow from player hand to this plug
    }

    public void EndPlug(GameObject targetPlug)
    {
        recieverPlug = targetPlug;
        isBeingPlugged = false;
        Debug.Log($"Plugged in a socket to {recieverPlug.transform.parent}");
    }

    public void CancelPlug()
    {
        isBeingPlugged = false;
        // Break VFX
    }

    public void ActivatePlug()
    {
        if(recieverPlug == null)
        {
            // Produce electricity damage thingy
        } else
        {
            recieverPlug.transform.parent.gameObject.GetComponent<BaseProp>().OnActivation();
        }
    }

}
