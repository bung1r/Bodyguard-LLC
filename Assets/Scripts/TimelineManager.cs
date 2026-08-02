using UnityEngine;
using System;

public class TimelineManager : MonoBehaviour
{

    public static Action OnSecondChanged;
    private RoundManager roundManager;

    public static int seconds { get; private set; }

    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = Time.time;
        roundManager = RoundManager.Instance;
        seconds = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // ? checks for if its null, shorthand instead of wrapping in an if conditional
        // OnSecondChanged?.Invoke();
        
        // slighly more reliable way to check a second. 
        if (Time.time - timer > 1f)
        {
            OnSecondChanged?.Invoke();
            timer = Time.time; 
            seconds++;
        }

        // if (timer <= 0)
        // {
        //     seconds++;
        //     timer = 1f;
        // }
    }
}
