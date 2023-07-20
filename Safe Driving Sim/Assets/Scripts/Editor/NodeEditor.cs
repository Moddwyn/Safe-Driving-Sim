using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeGroup))]
public class NodeEditor : Editor
{
    private bool isConnecting;
    private Group startGroup;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NodeGroup comp = (NodeGroup)target;
        Group group = comp.groups[comp.currentlyEditing];

        GUILayout.Space(10);

        if (!isConnecting)
        {
            if (GUILayout.Button("Start Connecting"))
            {
                isConnecting = true;
                startGroup = group;
            }
        }
        else
        {
            GUILayout.Label("Click on other nodes to connect to:");
            if (GUILayout.Button("Cancel"))
            {
                isConnecting = false;
                Debug.Log("Connection canceled");
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Clear List"))
        {
            group.nodes.Clear();
        }
    }

    private void OnSceneGUI()
    {
        if (isConnecting)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;

            if (e.type == EventType.MouseDown && e.button == 0 && Physics.Raycast(ray, out hit))
            {
                Node endNode = hit.collider.GetComponent<Node>();
                if (endNode != null)
                {
                    startGroup.nodes.Add(endNode);
                    Debug.Log("Connected Node: " + endNode.name);
                }
            }

            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape)
            {
                isConnecting = false;
                Debug.Log("Connection canceled");
            }
        }
    }
}
