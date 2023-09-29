using System.Collections.Generic;
using Barmetler.RoadSystem;
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(PathFinder))]
public class PathFinderEditor : Editor
{
    bool showBestPath = true;
    ReorderableList bestPathList;

    bool isConnectingStart;
    bool isConnectingEnd;

    GUIStyle unActiveButtonStyle;
    GUIStyle activeButtonStyle;

    void OnEnable()
    {
        bestPathList = new ReorderableList(serializedObject, serializedObject.FindProperty("bestPath"), true, true, true, true)
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Best Path")
        };
        bestPathList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = bestPathList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        unActiveButtonStyle = new GUIStyle(GUI.skin.button);
        unActiveButtonStyle.normal.background = MakeTex(2, 2, new Color(110f / 255, 110f / 255, 110f / 255));
        activeButtonStyle = new GUIStyle(GUI.skin.button);
        activeButtonStyle.normal.background = MakeTex(2, 2, new Color(125f / 255, 34f / 255, 27f / 255));

        PathFinder pathFinder = (PathFinder)target;

        EditorGUI.BeginDisabledGroup(true);
        showBestPath = EditorGUILayout.Foldout(showBestPath, "Best Path");
        if (showBestPath)
        {
            EditorGUI.indentLevel++;
            bestPathList.DoLayoutList();
            EditorGUI.indentLevel--;
        }
        EditorGUI.EndDisabledGroup();

        DrawHorizontalLine();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Start", GUILayout.Width(EditorGUIUtility.labelWidth - 80));
        pathFinder.start = EditorGUILayout.ObjectField(pathFinder.start, typeof(Node), true) as Node;
        if (!isConnectingStart)
        {
            if (GUILayout.Button("Find", unActiveButtonStyle, GUILayout.Width(50)))
            {
                isConnectingStart = true;
                isConnectingEnd = false;
            }
        }
        else
        {
            if (GUILayout.Button("Cancel", activeButtonStyle, GUILayout.Width(50)) || Event.current.keyCode == KeyCode.Escape)
            {
                isConnectingStart = false;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("End", GUILayout.Width(EditorGUIUtility.labelWidth - 80));
        pathFinder.end = EditorGUILayout.ObjectField(pathFinder.end, typeof(Node), true) as Node;
        if (!isConnectingEnd)
        {
            if (GUILayout.Button("Find", unActiveButtonStyle, GUILayout.Width(50)))
            {
                isConnectingEnd = true;
                isConnectingStart = false;
            }
        }
        else
        {
            if (GUILayout.Button("Cancel", activeButtonStyle, GUILayout.Width(50)) || Event.current.keyCode == KeyCode.Escape)
            {
                isConnectingEnd = false;
            }
        }
        GUILayout.EndHorizontal();

        DrawHorizontalLine();

        pathFinder.debugColor = EditorGUILayout.ColorField("Debug Color", pathFinder.debugColor);
        pathFinder.hideAllNodes = EditorGUILayout.Toggle("Hide All Nodes", pathFinder.hideAllNodes);

        GUILayout.Space(20);
        if (GUILayout.Button("Calulcate Path")) pathFinder.AutoCalculate(null);

        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        PathFinder pathFinder = (PathFinder)target;
        if (isConnectingStart)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;

            if (e.type == EventType.MouseDown && e.button == 0 && Physics.Raycast(ray, out hit))
            {
                Node endNode = hit.collider.GetComponent<Node>();
                if (endNode != null && endNode != pathFinder.start)
                {
                    pathFinder.start = endNode;
                    isConnectingStart = false;
                }
            }

            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape)
            {
                isConnectingStart = false;
            }
        }
        if (isConnectingEnd)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;

            if (e.type == EventType.MouseDown && e.button == 0 && Physics.Raycast(ray, out hit))
            {
                Node endNode = hit.collider.GetComponent<Node>();
                if (endNode != null && endNode != pathFinder.end)
                {
                    pathFinder.end = endNode;
                    isConnectingEnd = false;
                }
            }

            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape)
            {
                isConnectingEnd = false;
            }
        }
    }

    void DrawHorizontalLine()
    {
        GUILayout.Space(5);
        Rect rect = EditorGUILayout.GetControlRect(false, 1); // Create a 1-pixel tall rectangle
        EditorGUI.DrawRect(rect, Color.white); // Draw a colored line
        GUILayout.Space(5);
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
