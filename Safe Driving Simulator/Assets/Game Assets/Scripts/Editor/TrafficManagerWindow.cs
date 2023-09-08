using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TrafficManagerWindow : EditorWindow
{
    private string routeName = "Route";
    private string intersectionName = "Intersection";

    public Node nodePrefab;
    public IntersectionConnector intersectionPrefab;
    public bool showGizmoLines = true;
    bool prevShowLine;

    public List<CarAIController> carPrefabs = new List<CarAIController>();
    public static TrafficManagerWindow instance;

    private SerializedObject serializedObject;

    private Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Window/Traffic Manager")]
    private static void OpenWindow()
    {
        if (instance == null)
        {
            instance = GetWindow<TrafficManagerWindow>("Traffic Manager");
            instance.minSize = new Vector2(300, 250);
        }
        else
        {
            instance.Focus();
        }
    }

    private void OnEnable()
    {
        // Create a serialized object for the window
        serializedObject = new SerializedObject(this);
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneview)
    {
        Handles.BeginGUI();

        Vector2 newRoutePosition = new Vector2(
            sceneview.position.width * 0.5f - 50, // Center horizontally
            5 // A small offset from the top
        );
        Vector2 newIntersectionPosition = new Vector2(
            sceneview.position.width * 0.5f - 50, // Center horizontally
            newRoutePosition.y // A small offset from the top
        );

        // Begin a GUI area to control the button's position
        GUILayout.BeginArea(new Rect(newRoutePosition.x, newRoutePosition.y, 200, 50));

        if (GUILayout.Button("Add New Route", GUILayout.Width(200), GUILayout.Height(20)))
            AddNewRoute();
        if (GUILayout.Button("Add New Intersection", GUILayout.Width(200), GUILayout.Height(20)))
            AddNewIntersection();
        GUILayout.EndArea();



        Handles.EndGUI();
    }

    private void DrawHorizontalLine(Color color)
    {
        GUILayout.Space(5);
        Rect rect = EditorGUILayout.GetControlRect(false, 1); // Create a 1-pixel tall rectangle
        EditorGUI.DrawRect(rect, color); // Draw a colored line
        GUILayout.Space(5);
    }

    private void OnGUI()
    {
        serializedObject.Update();

        GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10);
        GUILayout.Label("Traffic Manager", headerStyle);

        GUILayout.Space(20);

        GUILayout.Label("Node Prefab:", EditorStyles.boldLabel);
        nodePrefab = EditorGUILayout.ObjectField(nodePrefab, typeof(Node), false) as Node;
        GUILayout.Label("Intersection Prefab:", EditorStyles.boldLabel);
        intersectionPrefab = EditorGUILayout.ObjectField(intersectionPrefab, typeof(IntersectionConnector), false) as IntersectionConnector;

        DrawHorizontalLine(Color.white);
        SerializedProperty carPrefabsProperty = serializedObject.FindProperty("carPrefabs");
        EditorGUILayout.PropertyField(carPrefabsProperty, true);
        DrawHorizontalLine(Color.white);

        GUILayout.Space(20);

        if (GUILayout.Button("Add New Road Manager"))
        {
            if (FindObjectOfType<RoadManager>() == null)
                AddNewRoadManager();
            else
                Debug.LogWarning("You already have an existing Road Manager");
        }
        if (GUILayout.Button("Add New Route"))
        {
            AddNewRoute();
        }
        if (GUILayout.Button("Add New Intersection"))
        {
            AddNewIntersection();
        }

        GUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();
    }

    private void AddNewRoute()
    {
        Route[] existingRoutes = FindObjectsOfType<Route>();
        int routeCount = existingRoutes.Length + 1;

        string newRouteName = routeName + " " + routeCount;
        GameObject newRoute = new GameObject(newRouteName);
        newRoute.AddComponent<Route>();
        newRoute.GetComponent<Route>().GenerateRandomID();
        Selection.activeObject = newRoute;
    }

    private void AddNewIntersection()
    {
        if (intersectionPrefab != null)
        {
            Vector3 newPosition = Vector3.zero;
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                Camera camera = sceneView.camera;
                Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of the view

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    newPosition = hit.point;
            }
            IntersectionConnector[] existingIntersection = FindObjectsOfType<IntersectionConnector>();
            int intersectionCount = existingIntersection.Length + 1;
            string newIntersectionName = intersectionName + " " + intersectionCount;

            IntersectionConnector newIntersection = Instantiate(intersectionPrefab, newPosition, Quaternion.identity);
            if (newIntersection != null)
            {

                newIntersection.gameObject.name = newIntersectionName;
                Selection.activeObject = newIntersection;
            }
            else
            {
                Debug.LogWarning("The prefab does not have a IntersectionConnector component.");
            }
        }
        else
        {
            Debug.LogWarning("Intersection prefab not found.");
        }
    }

    private void AddNewRoadManager()
    {
        GameObject newRoute = new GameObject("Road Manager");
        newRoute.AddComponent<RoadManager>();
        newRoute.GetComponent<RoadManager>().RefreshNodes();
        Selection.activeObject = newRoute;
    }
}