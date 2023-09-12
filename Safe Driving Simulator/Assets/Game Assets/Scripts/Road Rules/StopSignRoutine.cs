using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSignRoutine : MonoBehaviour
{
    public List<RoutinePattern> pattern = new List<RoutinePattern>();
    public float durationOnSwitch = 2;
    public float durationOnGo = 2;


    void Start()
    {
        StartCoroutine(StartPattern());
    }

    IEnumerator StartPattern()
    {
        while (true)
        {
            for (int i = 0; i < pattern.Count; i++)
            {
                List<StopSign> stoppedSigns = pattern[i].stoppedSigns;
                List<StopSign> goSigns = pattern[i].goSigns;
                foreach (var stopped in stoppedSigns) stopped.stop = true;

                foreach (var go in goSigns) go.stop = false;
                yield return new WaitForSeconds(durationOnGo);
                foreach (var go in goSigns) go.stop = true;
                yield return new WaitForSeconds(durationOnSwitch);
            }

            yield return null;
        }
        
    }

    [System.Serializable]
    public class RoutinePattern
    {
        public List<StopSign> stoppedSigns = new List<StopSign>();
        public List<StopSign> goSigns = new List<StopSign>();
    }
}
