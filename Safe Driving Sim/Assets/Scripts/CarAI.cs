using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{

    public Node[] nodes;

    public Vector2 speedRange;
    public float turnSmoothness = 1f;

    public int currIndex = 0;

    public float speed = 1f;
    [HideInInspector] public float s;
    public bool reverse;

    private void Awake()
    {
        speed = Random.Range(speedRange.x, speedRange.y);
        s = speed;
    }

    private void Update()
    {
        CarMovement();
    }

    private void CarMovement() {

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

        if (currIndex >= nodes.Length) {
            currIndex = 0;
        }
    }

    public void Test()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Collision") && other.transform.root != transform) {
            speed = -1;
            reverse = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Collision") && other.transform.root != transform)
        {
            speed = s;
            reverse = false;
        }
    }
}
