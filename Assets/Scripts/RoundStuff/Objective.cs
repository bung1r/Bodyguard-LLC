using TMPro;
using UnityEngine;

public class Objective : MonoBehaviour
{
    

    public ObjectiveSO objectiveSO;
    private float progress = 0f;
    private bool isWorking = false;
    [SerializeField] private bool isComplete = false;

    void Update()
    {
        if (isWorking && !isComplete)
        {
            // increase progress if isWorking!
            progress = Mathf.Min(progress + Time.deltaTime, objectiveSO.objectiveLength);
            if (progress >= objectiveSO.objectiveLength)
            {
                isComplete = true;
            }
        }
    }

    // ---- SETTERS -------
    public void SetProgress(float value) => progress = value;
    public void SetIsWorking(bool value) => isWorking = value;
    public void SetIsComplete(bool value) => isComplete = value;
    // ---- GETTERS -------
    public float GetProgress() => progress;
    public bool GetIsWorking() => isWorking;
    public bool GetIsComplete() => isComplete;
}