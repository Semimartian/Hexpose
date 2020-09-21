using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamingEnemy : Enemy
{
    [SerializeField] private float force;

    [SerializeField] private Rigidbody rigidbody;
    private Transform myTransform;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        Vector3 direction = myTransform.forward;
        Vector3 force = direction * this.force;
        rigidbody.AddForce(force, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {

        /*if (rigidbody.velocity.y > 0)
        {
            rigidbody.velocity = rigidbody.velocity - (Vector3.up * rigidbody.velocity.y);
        }*/
        rigidbody.velocity = rigidbody.velocity.normalized * force;
        Vector3 currentPosition = myTransform.position;
        myTransform.LookAt(currentPosition + rigidbody.velocity.normalized);
    }
}
