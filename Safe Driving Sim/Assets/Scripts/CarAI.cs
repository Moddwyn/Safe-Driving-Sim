using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    public bool resetAtStart;
    public bool randomGroup;
    [HideIf("WantsRandomGroup")] public int group;
    [SerializeField][HideIf("WantsRandomGroup")] List<Node> nodes;
    [ReadOnly] public int currIndex = 0;
    [HorizontalLine]
    public Vector2 speedRange;
    public float turnSmoothness = 1f;
    
    [HorizontalLine]
    [ReadOnly] public float speed = 1f;
    [ReadOnly] public float currentSpeed;
    [ReadOnly] public bool reverse;

    void Awake()
    {
        speed = Random.Range(speedRange.x, speedRange.y);
        currentSpeed = speed;
    }

    void Start()
    {
        if(WantsRandomGroup())
        {
            nodes.Clear();
            nodes = NodeGroup.Instance.GrabRandomGroup();
        } else
        {
            if(nodes.Count == 0)
                nodes = NodeGroup.Instance.GrabGroup(group);
        }
        if(resetAtStart) ResetCar();
    }

    void Update()
    {
        CarMovement();
    }

    void CarMovement() {
        if(nodes.Count == 0) return;

        Vector3 targetPosition = new Vector3(nodes[currIndex].transform.position.x, transform.root.position.y, nodes[currIndex].transform.position.z);
        Quaternion targetRotation = transform.root.rotation;

        if (Mathf.Abs(transform.root.position.x - targetPosition.x) > 0.1f || Mathf.Abs(transform.root.position.z - targetPosition.z) > 0.1f) {
            Quaternion lookRotation = Quaternion.LookRotation(targetPosition - transform.root.position);
            targetRotation = Quaternion.Slerp(transform.root.rotation, lookRotation, turnSmoothness * Time.deltaTime);
            targetRotation = Quaternion.Euler(transform.root.rotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.root.rotation.eulerAngles.z);
            transform.root.rotation = targetRotation;
            transform.root.position = Vector3.MoveTowards(transform.root.position, targetPosition, Time.deltaTime * speed);
        }

        if (Mathf.Abs(transform.root.position.x - targetPosition.x) < 0.1f && Mathf.Abs(transform.root.position.z - targetPosition.z) < 0.1f) {
            currIndex++;
        }

        if (currIndex >= nodes.Count) {
            currIndex = 0;
        }
    }

    public void ResetCar()
    {
        currIndex = 0;

        if(nodes.Count == 0) return;
        Vector3 targetPosition = new Vector3(nodes[currIndex].transform.position.x, transform.root.position.y, nodes[currIndex].transform.position.z);
        transform.root.position = targetPosition;
    }

    bool WantsRandomGroup() => randomGroup;
}
