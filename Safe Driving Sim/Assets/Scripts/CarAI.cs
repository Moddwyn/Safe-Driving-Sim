using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    public CarController carController;

    public Node[] nodes;

    public float speed = 1f;

    private int currIndex = -1;

    private void Update()
    {
        CarMovement();
        carController.horizontalInput *= speed;
        carController.verticalInput *= speed;
    }

    private void CarMovement() {
        Vector3 direction = (transform.position - nodes[currIndex + 1].transform.position).normalized;
        if (direction.z != 0) {
            carController.verticalInput = direction.z;
        }
        if (direction.x != 0) {
            carController.horizontalInput = direction.x;
            carController.verticalInput = direction.z;
        }
    }

}
