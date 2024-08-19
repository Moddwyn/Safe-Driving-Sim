using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class ScenerioManager : MonoBehaviour
{
    public List<Scenerio> scenerios = new List<Scenerio>();
    public UnityEvent OnReachEnd;
    public Transform endNodeParticle;
    [ReadOnly] public Node endNode;
    [ReadOnly] public bool hasReachedEnd;
    public static string scenario;
    private int scenarioIndex;

    [Button]
    void ChangeScenario() {
        StartScenarioByName("Neighborhood Safety");
    } 

    private void Start()
    {
        if (scenario == "") {
            return;
        }
        StartScenarioByName(scenario);
    }

    public void StartScenarioByName(string scenerioName)
    {
        for (int i = 0; i < scenerios.Count; i++) {
            if (scenerios[i].scenerioName == scenerioName)
            {
                StartScenario(scenerios[i]);
                break;
            }
        }
    }

    public void SetScenario(string scenarioName) {
        scenario = scenarioName;
    }

    private void StartScenario(Scenerio scenerio) {
        PlayerCar player = PlayerCar.Instance;
        player.transform.position = scenerio.start.transform.position;
        endNode = scenerio.end;
        endNodeParticle.transform.position = endNode.transform.position;
        player.transform.rotation = Quaternion.Euler(0, scenerio.angleStart, 0);
        player.GetComponent<PlayerCarUI>().target = endNode.transform;
        scenerio.OnSwitch?.Invoke();
    }

    public void RestartScenario() {
        StartScenario(scenerios[scenarioIndex]);
    }

    public void NextScenario() {
        scenarioIndex++;
        StartScenario(scenerios[scenarioIndex]);
    }

    [System.Serializable]
    public class Scenerio
    {
        public string scenerioName;
        public Node start;
        public Node end;
        public float angleStart = 90;
        public UnityEvent OnSwitch;
    }
}
