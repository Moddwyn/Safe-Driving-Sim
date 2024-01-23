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
    public TMP_Text timeTextWin;

    [HorizontalLine]
    public TMP_Text causeText;
    public TMP_Text speedText;
    public TMP_Text pointText;
    public TMP_Text averageSpeedText;
    public TMP_Text averageSpeedTextWin;
    public TMP_Text speedLimitText;
    public TMP_Text gamePointsText;
    [ReadOnly] public string cause;

    [HorizontalLine]
    public RectTransform directionArrow;
    public Transform target;

    PlayerCar player;
    PointSystem pointSystem;

    void Awake()
    {
        player = GetComponent<PlayerCar>();
        pointSystem = GetComponent<PointSystem>();
    }

    void Update()
    {
        speedText.text = (int)player.currentSpeed + " MPH";
        pointText.text = "Points: " + pointSystem.points.ToString();
        gamePointsText.text = "Points: " + pointSystem.points.ToString();
        UpdateFailUI();
        UpdateTimer();
        UpdateDirectionArrow();
    }
    
    void UpdateDirectionArrow()
    {
        if (target != null)
        {
            Vector3 directionToTarget = target.position - transform.position;
            float angleToTarget = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
            directionArrow.rotation = Quaternion.Euler(0, 0, -angleToTarget);
        }
    }
    
    void UpdateFailUI()
    {
        TimeSpan t = TimeSpan.FromSeconds(timer);
        string minutes = t.Minutes < 10 ? "0" + t.Minutes : t.Minutes.ToString();
        string seconds = t.Seconds < 10 ? "0" + t.Seconds : t.Seconds.ToString();
        timeText.text = "Time: " + minutes + ":" + seconds;
        timeTextWin.text = "Time: " + minutes + ":" + seconds;


        averageSpeedText.text = "Average Speed: " + (player.speedList.Any() ? player.speedList.Average() : 0).ToString("F2") + " mph";
        averageSpeedTextWin.text = "Average Speed: " + (player.speedList.Any() ? player.speedList.Average() : 0).ToString("F2") + " mph";
        causeText.text = "Cause: " + cause;
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

    void OnTriggerEnter(Collider other) {
        if(other.TryGetComponent<Node>(out Node node))
        {
            if(node.route != null) speedLimitText.text = node.route.maxMPH+"";
        }
    }
}
