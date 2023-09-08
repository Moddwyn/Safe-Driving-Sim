using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class TrafficLightRoutine : MonoBehaviour
{
    public List<RoutinePattern> pattern = new List<RoutinePattern>();

    [HorizontalLine]
    public RoutineTimers routineTimers;

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
                List<TrafficLight> activeList = pattern[i].activeLights;
                List<TrafficLight> inactiveList = pattern[i].inactiveLights;
                foreach (var inactiveLights in inactiveList) inactiveLights.Red();

                float durationOnGreen = !pattern[i].overrideTimers? routineTimers.durationOnGreen : pattern[i].timers.durationOnGreen;
                float durationOnYellow = !pattern[i].overrideTimers? routineTimers.durationOnYellow : pattern[i].timers.durationOnYellow;
                float durationOnChange = !pattern[i].overrideTimers? routineTimers.durationOnChange : pattern[i].timers.durationOnChange;

                foreach (var activeLights in activeList) activeLights.Green();
                yield return new WaitForSeconds(durationOnGreen);
                foreach (var activeLights in activeList) activeLights.Yellow();
                yield return new WaitForSeconds(durationOnYellow);
                foreach (var activeLights in activeList) activeLights.Red();
                yield return new WaitForSeconds(durationOnChange);
            }

            yield return null;
        }
        
    }

    [System.Serializable]
    public class RoutinePattern
    {
        public List<TrafficLight> activeLights = new List<TrafficLight>();
        public List<TrafficLight> inactiveLights = new List<TrafficLight>();
        
        public bool overrideTimers;
        [ShowIf("overrideTimers")][AllowNesting] public RoutineTimers timers;
    }

    [System.Serializable]
    public class RoutineTimers
    {
        public float durationOnGreen = 5;
        public float durationOnYellow = 3;
        public float durationOnChange = 4;
    }
}
