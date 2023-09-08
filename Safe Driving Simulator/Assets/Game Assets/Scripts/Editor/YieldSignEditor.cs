using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(YieldSigns))]
public class YieldSignEditor : Editor
{
    bool isChoosingStopNode;

    YieldSigns cachedSign;

    private void OnEnable()
    {
        cachedSign = (YieldSigns)target;
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
            GUILayout.Label("Click a node you want cars to stop on when cars are passing by:");
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

                if(cachedSign.stopNode != endNode)
                    cachedSign.stopNode = endNode;

                isChoosingStopNode = false;
            }

            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape)
            {
                isChoosingStopNode = false;
            }
        }
    }
}