using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGroup : MonoBehaviour
{
    public List<Group> groups;
    public int currentlyEditing;

    public static NodeGroup Instance;

    void Awake()
    {
        Instance = this;
    }

    public List<Node> GrabRandomGroup() => GrabGroup(Random.Range(0, groups.Count));
    public List<Node> GrabGroup(int num) => groups[num].nodes;
}

[System.Serializable]
public class Group
{
    public List<Node> nodes;
}