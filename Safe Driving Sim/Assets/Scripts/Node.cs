using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public NodeType nodeType;

    public enum NodeType { 
        Normal, Stop
    }
}
