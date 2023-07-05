using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarUI : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    private CarController carController;

    private void Awake()
    {
        carController = GetComponent<CarController>();
    }

    private void Update()
    {
        speedText.text = Mathf.Round((carController.currentSpeed * 3.6f)).ToString() + " km/h";
    }
}
