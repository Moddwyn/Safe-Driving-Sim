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

    private void Start() {
        StartScenerio("Neighborhood Safety");
    }

    public void StartScenerio(string scenerioName)
    {
        PlayerCar player = PlayerCar.Instance;
        foreach (var s in scenerios)
        {
            if(s.scenerioName == scenerioName)
            {
                player.transform.position = s.start.transform.position;

                endNode = s.end;
                endNodeParticle.transform.position = endNode.transform.position;
                player.transform.rotation = Quaternion.Euler(0, s.angleStart, 0);
                player.GetComponent<PlayerCarUI>().target = endNode.transform;
            }
        }
    }

    [System.Serializable]
    public class Scenerio
    {
        public string scenerioName;
        public Node start;
        public Node end;
        public float angleStart = 90;
    }
}
