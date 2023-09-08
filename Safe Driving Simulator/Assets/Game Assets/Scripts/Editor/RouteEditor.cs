using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
[CustomEditor(typeof(Route))]
public class RouteEditor : Editor
{
    private bool isAddingNodes;
    private TrafficManagerWindow trafficManagerWindow;
    private Route cachedRoute;
    private Node previewNode;

    private SerializedProperty nodesProperty;
    private SerializedProperty carsProperty;
    private SerializedProperty colorProperty;

    private void OnEnable()
    {
        trafficManagerWindow = FindExistingTrafficManagerWindow();
        cachedRoute = (Route)target; // Cache the Route object

        colorProperty = serializedObject.FindProperty("defaultLineColor");
        nodesProperty = serializedObject.FindProperty("nodes");
        carsProperty = serializedObject.FindProperty("cars");
    }

    private TrafficManagerWindow FindExistingTrafficManagerWindow()
    {
        return Resources.FindObjectsOfTypeAll<TrafficManagerWindow>().FirstOrDefault();
    }

    private void DrawHorizontalLine(Color color)
    {
        GUILayout.Space(5);
        Rect rect = EditorGUILayout.GetControlRect(false, 1); // Create a 1-pixel tall rectangle
        EditorGUI.DrawRect(rect, color); // Draw a colored line
        GUILayout.Space(5);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Update serialized object

        Route route = cachedRoute; // Use the cached Route object

        GUILayout.Space(10);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.IntField("ID", route.id);
        EditorGUI.EndDisabledGroup();

        // Display maxMPH field as usual
        route.maxMPH = EditorGUILayout.IntField("Max MPH", route.maxMPH);
        route.defaultLineColor = EditorGUILayout.ColorField("Default Line Color", route.defaultLineColor);

        
        // Draw the "Nodes" label centered and bold
        GUIStyle centeredBoldStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        DrawHorizontalLine(Color.white);

        EditorGUILayout.LabelField("Cars", centeredBoldStyle);
        for (int i = 0; i < carsProperty.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(carsProperty.GetArrayElementAtIndex(i), GUIContent.none);
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                carsProperty.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (carsProperty.arraySize < route.nodes.Count-1)
        {
            if (GUILayout.Button("Add Car"))
            {
                int newIndex = carsProperty.arraySize;
                carsProperty.arraySize++;
                if (newIndex > 0)
                {
                    SerializedProperty previousCar = carsProperty.GetArrayElementAtIndex(newIndex - 1);
                    SerializedProperty newCar = carsProperty.GetArrayElementAtIndex(newIndex);
                    newCar.objectReferenceValue = previousCar.objectReferenceValue;
                } else
                {
                    SerializedProperty newCar = carsProperty.GetArrayElementAtIndex(newIndex);
                    if(trafficManagerWindow.carPrefabs.Count > 0)
                        newCar.objectReferenceValue = trafficManagerWindow.carPrefabs[Random.Range(0, trafficManagerWindow.carPrefabs.Count)];
                    else
                        Debug.LogWarning("Please add car prefabs to the Traffic Manager window");
                }
            }
        }

        DrawHorizontalLine(Color.white);
    
        GUILayout.Space(20);
        route.connectNodes = EditorGUILayout.Toggle("Connect Nodes", route.connectNodes);
        EditorGUILayout.LabelField("Nodes", centeredBoldStyle);
        if (!isAddingNodes)
        {
            if (nodesProperty.arraySize == 0)
            {
                GUILayout.Label("Empty", centeredBoldStyle); // Display "Empty" label when list is empty
            }
            else
            {
                GUILayout.Space(5);

                // Draw the "nodes" list manually
                for (int i = 0; i < nodesProperty.arraySize; i++)
                {
                    SerializedProperty nodeProperty = nodesProperty.GetArrayElementAtIndex(i);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(nodeProperty, GUIContent.none);
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        RemoveNodeFromListAndDestroyGameObject(i, cachedRoute);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                // Apply any property changes
                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Add Nodes"))
            {
                isAddingNodes = true;
            }
        }
        else
        {
            GUILayout.Label("Click in the scene to place nodes:");
            if (GUILayout.Button("Cancel") || Event.current.keyCode == KeyCode.Escape)
            {
                isAddingNodes = false;
            }
        }

        if (GUILayout.Button("Clear Nodes List"))
        {
            ClearNodesList(route);
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
                AddNodeToRoute(hit.point, cachedRoute);
            }

            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape)
            {
                isAddingNodes = false;
            }
        }
    }
    private void DrawDefaultInspectorFields()
    {
        EditorGUI.BeginChangeCheck();
        serializedObject.Update(); // Update serialized object

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
            {
                EditorGUILayout.PropertyField(iterator, true);
            }
            enterChildren = false;
        }

        // Apply any property changes
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
    private void RemoveNodeFromListAndDestroyGameObject(int index, Route route)
    {
        if (index >= 0 && index < nodesProperty.arraySize)
        {
            SerializedProperty nodeProperty = nodesProperty.GetArrayElementAtIndex(index);
            Node node = nodeProperty.objectReferenceValue as Node;

            if (node != null)
            {
                // Get the previous node if it exists
                Node previousNode = index > 0 ? nodesProperty.GetArrayElementAtIndex(index - 1).objectReferenceValue as Node : null;

                // Get the next node if it exists
                Node nextNode = index < nodesProperty.arraySize - 1 ? nodesProperty.GetArrayElementAtIndex(index + 1).objectReferenceValue as Node : null;

                if (previousNode != null)
                {
                    // Remove the node from the previous node's connectedNodes list
                    previousNode.RemoveNode(node);

                    // If there's a next node, update the connection from the previous node to the next node
                    if (nextNode != null && route.connectNodes)
                    {
                        previousNode.AddNewNode(nextNode);
                    }
                }

                // Remove the node from the list
                nodesProperty.DeleteArrayElementAtIndex(index);

                // Destroy the corresponding game object
                DestroyImmediate(node.gameObject);

                // Apply the changes
                serializedObject.ApplyModifiedProperties();
            }
        }
    }



    private void AddNodeToRoute(Vector3 position, Route route)
    {
        if (trafficManagerWindow != null && trafficManagerWindow.nodePrefab != null)
        {
            Node newNode = Instantiate(trafficManagerWindow.nodePrefab, position, Quaternion.identity);
            newNode.route = route;

            if (newNode != null)
            {
                int newNodeNumber = route.nodes.Count + 1; // Calculate the new node number

                // Assign the random number-based name
                newNode.name = "Node " + newNodeNumber;

                route.nodes.Add(newNode);

                // Connect the new node to the preview node if available
                if (previewNode != null && route.connectNodes)
                {
                    previewNode.AddNewNode(newNode);
                }
                else
                {
                    if (route.nodes.Count > 1)
                    {
                        previewNode = route.nodes[route.nodes.Count - 2];
                        if(route.connectNodes) previewNode.AddNewNode(newNode);
                    }
                }

                newNode.transform.SetParent(route.transform);
                // Set the new node as the preview node
                previewNode = newNode;
            }
            else
            {
                Debug.LogWarning("The prefab does not have a Node component.");
            }
        }
        else
        {
            Debug.LogWarning("Node prefab not found.");
        }
    }
    
    private void ClearNodesList(Route route)
    {
        foreach (Node node in route.nodes)
        {
            if (node != null)
            {
                DestroyImmediate(node.gameObject);
            }
        }

        route.nodes.Clear();
    }
}