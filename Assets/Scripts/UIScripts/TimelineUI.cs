using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimelineUI : MonoBehaviour
{
    public static TimelineUI Instance;
    private RoundManager roundManager;
    public Image mainTimeline;
    public Image timePointer;
    public Image tickModel;
    List<Image> checkpointTicks = new List<Image>(); 
    public TextMeshProUGUI startSecondsUI;
    public TextMeshProUGUI endSecondsUI;
    public float roundStartTime = 0f;
    public float roundEndTime = 300f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        roundManager = RoundManager.Instance;
        startSecondsUI.text = roundStartTime.ToString() + "s";
        endSecondsUI.text = roundEndTime.ToString() + "s";
    }

    void Update()
    {
        UpdateTimePointer();
    }

    public void UpdateTimePointer()
    {

        timePointer.rectTransform.localPosition = new Vector3(
            CalcXOnTimeline(),
            20,
            0
        );
        // timePointer.rectTransform.localPosition = new Vector3(
        //     locposition.x * ratio,
        //     timePointer.transform.position.y,
        //     timePointer.transform.position.z
        // );
    }
    public float CalcXOnTimeline()
    {
        float ratio = (roundManager.roundTime / roundEndTime) - 0.5f;
        // ratio of time into the round, from range -0.5 -> 0.5
        return mainTimeline.rectTransform.sizeDelta.x * ratio;
    }

    public void CreateTick(TimelineTickTypes type)
    {
        Color color = new Color32(255,255,255,255);
        Image tick = Instantiate(tickModel, mainTimeline.transform);
        tick.rectTransform.localPosition = new Vector3(CalcXOnTimeline(), 0, 0);
        switch(type)
        {
            case TimelineTickTypes.FixedCheckpoint:
                color = new Color32(120, 120, 120, 255);
                tick.color = color;
                checkpointTicks.Add(tick);
                break;
            case TimelineTickTypes.FlexibleCheckpoint:
                color = new Color32(255, 235, 30, 255);
                tick.color = color;
                checkpointTicks.Add(tick);
                break;
        }
        
    }
    
    public void RemoveLastTick(TimelineTickTypes type)
    {
        switch(type)
        {
            case TimelineTickTypes.FixedCheckpoint:
            case TimelineTickTypes.FlexibleCheckpoint:
                Image tick = checkpointTicks[checkpointTicks.Count - 1];
                checkpointTicks.RemoveAt(checkpointTicks.Count - 1);
                Destroy(tick);
                break;
        }

        
    }


}



public enum TimelineTickTypes
{
    FixedCheckpoint,
    FlexibleCheckpoint
}