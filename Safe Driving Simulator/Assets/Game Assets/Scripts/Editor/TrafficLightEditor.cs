using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrafficLight))]
public class TrafficLightEditor : Editor
{
    bool isChoosingStopNode;

    TrafficLight cachedLight;

    private void OnEnable()
    {
        cachedLight = (TrafficLight)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(20);
        ConnectStopNodeButton();

        serializedObject.ApplyModifiedProperties();
    }

    void ConnectStopNodeButton()
    {
        if (!isChoosingStopNode)
        {
            if (GUILayout.Button("Choose Stop Node"))
            {
                isChoosingStopNode = true;
            }
        }
        else
        {
            GUILayout.Label("Click a node you want cars to stop on when light is not green:");
            if (GUILayout.Button("Cancel") || Event.current.keyCode == KeyCode.Escape)
            {
                isChoosingStopNode = false;
            }
        }
    }

    void OnSceneGUI()
    {
        if (isChoosingStopNode)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;

            if (e.type == EventType.MouseDown && e.button == 0 && Physics.Raycast(ray, out hit))
            {
                Node endNode = hit.collider.GetComponent<Node>();

                if(cachedLight.stopNode != endNode)
                    cachedLight.stopNode = endNode;

                isChoosingStopNode = false;
            }

            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape)
            {
                isChoosingStopNode = false;
            }
        }
    }
}