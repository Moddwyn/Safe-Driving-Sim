using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarUI : MonoBehaviour
{
    public TextMeshProUGUI speedText, centeredText, wrongWayText;
    private CarController carController;

    private void Awake()
    {
        carController = GetComponent<CarController>();
    }

    private void Update()
    {
        speedText.text = Mathf.Round((carController.currentSpeed)).ToString() + " km/h";
        centeredText.enabled = !carController.isCentered;
        wrongWayText.enabled = !carController.facingLane;
    }
}
