using UnityEngine;
using System;

public class TimelineManager : MonoBehaviour
{

    public static Action OnSecondChanged;

    public static int seconds { get; private set; }

    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        seconds = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        // ? checks for if its null, shorthand instead of wrapping in an if conditional
        OnSecondChanged?.Invoke();
        
        if (timer <= 0)
        {
            seconds++;
            timer = 1f;
        }
    }
}
