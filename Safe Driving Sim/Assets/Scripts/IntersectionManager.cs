using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class IntersectionManager : MonoBehaviour
{
    public List<RoutinePattern> patterns;

    public TrafficLight nwLight;
    public TrafficLight neLight;
    public TrafficLight swLight;
    public TrafficLight seLight;

    public enum LightDirection { NW, NE, SW, SE };
    [HorizontalLine]
    public float changeIntervals;
    public float yellowIdle;

    [HorizontalLine]
    public TMP_Text nwText;
    public TMP_Text neText;
    public TMP_Text swText;
    public TMP_Text seText;
    public Color greenColor;
    public Color yellowColor;
    public Color redColor;
    public bool showTexts;
    [ReadOnly] public int currentPattern;

    void Start()
    {
        if (nwText != null)
        {
            nwText.gameObject.SetActive(showTexts);
            neText.gameObject.SetActive(showTexts);
            swText.gameObject.SetActive(showTexts);
            seText.gameObject.SetActive(showTexts);
        }
        StartCoroutine(TrafficRoutine());
    }

    void Update()
    {
        if (nwText != null)
        {
            nwText.color = nwLight.trafficColor == TrafficLight.LightColor.Red ? redColor :
                                        nwLight.trafficColor == TrafficLight.LightColor.Yellow ? yellowColor : greenColor;

            neText.color = neLight.trafficColor == TrafficLight.LightColor.Red ? redColor :
                                        neLight.trafficColor == TrafficLight.LightColor.Yellow ? yellowColor : greenColor;

            swText.color = swLight.trafficColor == TrafficLight.LightColor.Red ? redColor :
                                        swLight.trafficColor == TrafficLight.LightColor.Yellow ? yellowColor : greenColor;

            seText.color = seLight.trafficColor == TrafficLight.LightColor.Red ? redColor :
                                        seLight.trafficColor == TrafficLight.LightColor.Yellow ? yellowColor : greenColor;
        }
        if (Input.GetKeyDown(KeyCode.T)) Time.timeScale = 3;
    }

    IEnumerator TrafficRoutine()
    {
        TrafficLight dirLight;
        foreach (var l in patterns[currentPattern].lightActions)
        {
            if (l.light == LightDirection.NW) dirLight = nwLight;
            else if (l.light == LightDirection.NE) dirLight = neLight;
            else if (l.light == LightDirection.SW) dirLight = swLight;
            else dirLight = seLight;
            dirLight.trafficColor = l.color;
            dirLight.inverseColor = l.inverseColor;
        }

        yield return new WaitForSeconds(changeIntervals);
        foreach (var l in patterns[currentPattern].lightActions)
        {
            if (l.light == LightDirection.NW) dirLight = nwLight;
            else if (l.light == LightDirection.NE) dirLight = neLight;
            else if (l.light == LightDirection.SW) dirLight = swLight;
            else dirLight = seLight;
            if (dirLight.trafficColor == TrafficLight.LightColor.Green)
            {
                dirLight.trafficColor = TrafficLight.LightColor.Yellow;
            }
        }
        yield return new WaitForSeconds(yellowIdle);

        currentPattern++;
        if (currentPattern >= patterns.Count) currentPattern = 0;
        StartCoroutine(TrafficRoutine());
    }
}

[System.Serializable]
public class RoutinePattern
{
    public List<LightAction> lightActions;
}

[System.Serializable]
public class LightAction
{
    public IntersectionManager.LightDirection light;
    public TrafficLight.LightColor color;
    public TrafficLight.LightColor inverseColor;
}
