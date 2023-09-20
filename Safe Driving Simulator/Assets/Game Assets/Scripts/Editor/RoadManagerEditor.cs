using UnityEditor;
using UnityEngine;
using NaughtyAttributes.Editor;

[CustomEditor(typeof(RoadManager))]
public class RoadManagerEditor : UnityEditor.Editor
{
    private SerializedProperty allNodesProp;
    private SerializedProperty waypointNodesProp;
    private SerializedProperty trafficCheckRadProp;
    private SerializedProperty carCheckRadProp;

    private bool isAddingNodes;
    private RoadManager cachedRoad;

    private void OnEnable()
    {
        cachedRoad = (RoadManager)target;

        allNodesProp = serializedObject.FindProperty("allNodes");
        waypointNodesProp = serializedObject.FindProperty("waypointNodes");
        trafficCheckRadProp = serializedObject.FindProperty("trafficCheckRad");
        carCheckRadProp = serializedObject.FindProperty("carCheckRad");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawNodesGroup();
        EditorGUILayout.Space();
        DrawTrafficSettingsGroup();
        EditorGUILayout.Space();
        DrawRefreshButton();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawNodesGroup()
    {
        RoadManager road = cachedRoad;

        GUIStyle centeredBoldStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        
        EditorGUILayout.PropertyField(allNodesProp, true);
        
        EditorGUILayout.LabelField("Waypoint Nodes", centeredBoldStyle);
        if (!isAddingNodes)
        {
            if (waypointNodesProp.arraySize == 0)
            {
                GUILayout.Label("Empty", centeredBoldStyle); // Display "Empty" label when list is empty
            }
            else
            {
                GUILayout.Space(5);

                // Draw the "nodes" list manually
                for (int i = 0; i < waypointNodesProp.arraySize; i++)
                {
                    SerializedProperty nodeProperty = waypointNodesProp.GetArrayElementAtIndex(i);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(nodeProperty, GUIContent.none);
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        RemoveWaypoint(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                // Apply any property changes
                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Add Waypoints"))
            {
                isAddingNodes = true;
            }
        }
        else
        {
            GUILayout.Label("Click nodes on the scene to select waypoints:");
            if (GUILayout.Button("Cancel") || Event.current.keyCode == KeyCode.Escape)
            {
                isAddingNodes = false;
            }
        }

        if (GUILayout.Button("Clear Waypoints"))
        {
            ClearWaypoints(road);
        }
    }

    private void DrawTrafficSettingsGroup()
    {
        EditorGUILayout.LabelField("Traffic Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(trafficCheckRadProp);
        EditorGUILayout.PropertyField(carCheckRadProp);
    }

    private void DrawRefreshButton()
    {
        if (GUILayout.Button("Refresh"))
        {
            RoadManager roadManager = (RoadManager)target;
            roadManager.RefreshNodes();
        }
    }

    private void ClearWaypoints(RoadManager road)
    {
        road.waypointNodes.Clear();
    }

    private void RemoveWaypoint(int index)
    {
        if (index >= 0 && index < waypointNodesProp.arraySize)
        {
            // Remove the node from the list
            waypointNodesProp.DeleteArrayElementAtIndex(index);

            // Apply the changes
            serializedObject.ApplyModifiedProperties();
        }
    }
    
    private void OnSceneGUI()
    {
        if (isAddingNodes)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;

            if (e.type == EventType.MouseDown && e.button == 0 && Physics.Raycast(ray, out hit))
            {
                Node clickedNode = hit.collider.GetComponent<Node>();

                if(!cachedRoad.waypointNodes.Contains(clickedNode) && clickedNode != null)
                    cachedRoad.waypointNodes.Add(clickedNode);
            }

            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape)
            {
                isAddingNodes = false;
            }
        }
    }
}
