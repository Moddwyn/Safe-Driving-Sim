using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{
    private int selectedTabIndex;
    private bool isConnecting;
    private Node startNode;

    private SerializedProperty nodeTypeProperty;
    private SerializedProperty layerMask;

    private void OnEnable()
    {
        selectedTabIndex = ((Node)target).nodeType == Node.NodeType.None ? 0 : 1;
        nodeTypeProperty = serializedObject.FindProperty("nodeType");
        layerMask = serializedObject.FindProperty("carMask");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        Node node = (Node)target;

        EditorGUILayout.PropertyField(layerMask, new GUIContent("Car Layermask"));

        GUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("connectedNodes"), true);

        GUIStyle centeredBoldLabel = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        if (!isConnecting)
        {
            if (GUILayout.Button("Start Connecting"))
            {
                isConnecting = true;
                startNode = node;
            }
        }
        else
        {
            GUILayout.Label("Click on other nodes to connect to:");
            if (GUILayout.Button("Cancel") || Event.current.keyCode == KeyCode.Escape)
            {
                isConnecting = false;
            }
        }
        if (GUILayout.Button("Clear List"))
        {
            node.connectedNodes.Clear();
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Node Type", centeredBoldLabel);

        GUILayout.BeginHorizontal();

        GUIStyle noneStyle = new GUIStyle(GUI.skin.button);
        GUIStyle stopStyle = new GUIStyle(GUI.skin.button);

        if (selectedTabIndex == 0)
        {
            noneStyle.normal.textColor = Color.white;
            noneStyle.normal.background = MakeTex(2, 2, new Color(0.2f, 0.5f, 0.2f, 1f));
            stopStyle.normal.background = null;
        }
        else if (selectedTabIndex == 1)
        {
            stopStyle.normal.textColor = Color.white;
            stopStyle.normal.background = MakeTex(2, 2, new Color(0.5f, 0.2f, 0.2f, 1f));
            noneStyle.normal.background = null;
        }

        if (GUILayout.Button("None", noneStyle))
        {
            selectedTabIndex = 0;
            nodeTypeProperty.enumValueIndex = (int)Node.NodeType.None;
            serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("Stop", stopStyle))
        {
            selectedTabIndex = 1;
            nodeTypeProperty.enumValueIndex = (int)Node.NodeType.Stop;
            serializedObject.ApplyModifiedProperties();
        }

        GUILayout.EndHorizontal();

        node.DeleteEmptyNodes();

        serializedObject.ApplyModifiedProperties();
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
                if (endNode != null && endNode != startNode && !startNode.connectedNodes.Contains(endNode))
                {
                    startNode.connectedNodes.Add(endNode);
                    Selection.objects = new GameObject[1] { endNode.gameObject };
                }
            }

            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape)
            {
                isConnecting = false;
            }
        }
    }

    private Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = color;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
