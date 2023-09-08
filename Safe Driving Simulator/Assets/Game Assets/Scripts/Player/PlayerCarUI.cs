using System;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerCar))]
public class PlayerCarUI : MonoBehaviour
{
    [ReadOnly] public float timer;
    [ReadOnly] public bool startTimer;
    public TMP_Text timeText;

    [HorizontalLine]
    public TMP_Text causeText;
    public TMP_Text speedText;
    public TMP_Text averageSpeedText;
    [ReadOnly] public string cause;

    PlayerCar player;

    void Start()
    {
        player = GetComponent<PlayerCar>();
    }

    void Update()
    {
        speedText.text = (int)player.currentSpeed + " MPH";
        UpdateFailUI();
        UpdateTimer();
    }

    void UpdateFailUI()
    {
        TimeSpan t = TimeSpan.FromSeconds(timer);
        string minutes = t.Minutes < 10 ? "0" + t.Minutes : t.Minutes.ToString();
        string seconds = t.Seconds < 10 ? "0" + t.Seconds : t.Seconds.ToString();
        timeText.text = "Time: " + minutes + ":" + seconds;

        averageSpeedText.text = "Average Speed: " + (player.speedList.Any() ? player.speedList.Average() : 0) + " mph";
    }

    void UpdateTimer()
    {
        timer += startTimer? Time.deltaTime : 0;
    }

    [Button("Start or Resume Timer")]
    public void StartResumeTimer()
    {
        startTimer = true;
    }

    [Button("Stop Timer")]
    public void StopTimer()
    {
        startTimer = false;
    }

    [Button("Reset Timer")]
    public void ResetTimer()
    {
        timer = 0;
    }
}
