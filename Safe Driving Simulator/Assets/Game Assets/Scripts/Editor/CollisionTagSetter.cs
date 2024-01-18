using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CollisionTagSetter : MonoBehaviour
{
    [MenuItem("Custom/Set Colliders Tag/Collision")]
    private static void SetCollidersToCollisionTag()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject selectedObject in selectedObjects)
        {
            // Set the tag of the selected object if it has a Collider component
            Collider collider = selectedObject.GetComponent<Collider>();
            if (collider != null)
            {
                selectedObject.tag = "Collision";
            }

            // Set the tag for all child objects recursively
            SetChildCollidersToCollisionTag(selectedObject.transform);
        }
    }

    private static void SetChildCollidersToCollisionTag(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Collider collider = child.GetComponent<Collider>();
            if (collider != null)
            {
                child.gameObject.tag = "Collision";
            }

            // Recursively check children
            SetChildCollidersToCollisionTag(child);
        }
    }
}
