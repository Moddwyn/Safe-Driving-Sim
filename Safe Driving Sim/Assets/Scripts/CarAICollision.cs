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
    [ReadOnly][SerializeField] bool stuck;

    [ReadOnly][SerializeField] bool isCollidingTimer;
    [ReadOnly][SerializeField] bool isReversingTimer;

    BoxCollider boxCollider;
    [ReadOnly] public List<Collider> colliders;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        colliders = Physics.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.extents, Quaternion.identity).ToList();
        // colliding = colliders.Any(c => c.CompareTag("Collision") && c.transform.root != transform && !car.reverse);
        // stuck = colliders.Any(c=>c.transform.root != transform
        // && c.GetComponentInChildren<CarAICollision>()?.colliders.Any(m=>m.transform.root == transform.root) == true );
        colliding = colliders.Any(collider => collider.CompareTag("Collision") && collider.transform.root != transform.root && !car.reverse);

        stuck = colliders.Any(collider => collider.GetComponent<CarAICollision>() != null
        && collider.GetComponent<CarAICollision>() != this
        && collider.GetComponent<CarAICollision>().colliders.Contains(boxCollider));

        if (stuck)
        {
            if (colliding)
            {
                if (!isCollidingTimer)
                {
                    isCollidingTimer = true;
                    car.speed = 0;
                    StartCoroutine(CollisionWait());
                }
            }
            else
            {
                if (!isCollidingTimer && !isReversingTimer)
                {
                    car.speed = car.currentSpeed;
                }
                isCollidingTimer = false;
            }
        } else
        {
            if (!isReversingTimer)
                car.speed = colliding? 0 : car.currentSpeed;
        }
    }

    IEnumerator CollisionWait()
    {
        yield return new WaitForSeconds(3);
        if (!car.reverse && colliding)
        {
            car.speed = -1;
            StartCoroutine(ReverseWait());
        }
    }

    IEnumerator ReverseWait()
    {
        isReversingTimer = true;
        yield return new WaitForSeconds(Random.Range(2, 4));
        if (!car.reverse)
        {
            isReversingTimer = false;
        }
    }
}
