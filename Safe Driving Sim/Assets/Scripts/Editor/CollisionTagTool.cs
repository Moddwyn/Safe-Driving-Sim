using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CollisionTagTool : EditorWindow
{
    private string newTag = "Collision";

    [MenuItem("Tools/Change Child Tags")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CollisionTagTool));
    }

    private void OnGUI()
    {
        GUILayout.Label("Change Child Tags", EditorStyles.boldLabel);
        newTag = EditorGUILayout.TextField("New Tag:", newTag);

        if (GUILayout.Button("Apply"))
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            foreach (GameObject selectedObject in selectedObjects)
            {
                ChangeChildTags(selectedObject.transform);
            }
        }
    }

    private void ChangeChildTags(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.GetComponent<Collider>() != null)
            {
                child.tag = newTag;
            }

            if (child.childCount > 0)
            {
                ChangeChildTags(child);
            }
        }
    }
}
