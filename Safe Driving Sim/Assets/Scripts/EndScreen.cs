using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI averageSpeedText;


    private void OnEnable()
    {
        TimeSpan t = TimeSpan.FromSeconds(Time.time - FindObjectOfType<Level>().startTime);
        string minutes = t.Minutes < 10 ? "0" + t.Minutes : t.Minutes.ToString();
        string seconds = t.Seconds < 10 ? "0" + t.Seconds : t.Seconds.ToString();
        timeText.text = "Time: " + minutes + ":" + seconds;

        CarController carController = FindObjectOfType<CarController>();
        float totalSpeed = 0f;
        foreach (float speed in carController.speeds) {
            totalSpeed += speed;
        }
        averageSpeedText.text = (totalSpeed / carController.speeds.Count).ToString() + " km/h";
    }
}
