using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CarAICollision : MonoBehaviour
{
    public CarAI car;
    [ReadOnly][SerializeField] bool colliding;

    BoxCollider boxCollider;
    Collider[] colliders;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        colliders = Physics.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.extents, Quaternion.identity);
        colliding = colliders.Any(c => c.CompareTag("Collision") && c.transform.root != transform && !car.reverse);
        if(!car.reverse)
            car.speed = colliding ? 0 : car.currentSpeed;
    }
}
