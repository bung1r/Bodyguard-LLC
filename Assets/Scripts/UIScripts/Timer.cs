using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI text;
    public static Timer Instance;
    private bool isRunning = false;
    private float timerValue = 0f;
    public void BeginTimer()
    {
        isRunning = true;
    }
    public void StopTimer()
    {
        isRunning = false;
    }
    public void SetTimer(float seconds)
    {
        timerValue = Mathf.Round(seconds * 100f)/100f;
        text.text = timerValue.ToString("0.00");
        // rounds to the nearest 2 decimal places
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(Instance);
        }
    }
    void Update()
    {
        if (isRunning)
        {
            SetTimer(timerValue += Time.deltaTime);
        }
    }
}
