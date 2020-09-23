using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamingEnemy : Enemy
{
    [SerializeField] private float force;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem collisionParticles;
    private Transform myTransform;

    void Start()
    {
        myTransform = transform;
        Vector3 direction = myTransform.forward;
        Vector3 force = direction * this.force;
        rigidbody.AddForce(force, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (GameManager.GameOver || !isAlive)
        {
            rigidbody.velocity = Vector3.zero;
        }
        else
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


    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.GameOver|| !isAlive)
        {
            return;
        }
        //
        if (collision.collider != null )
        {
           // Debug.Log(collision.gameObject.name);
            DoCollisionViewStuff();     
        }
    }

    private int frameCount;
    private void DoCollisionViewStuff()
    {
        int currentFrameCount = Time.frameCount;
        if (currentFrameCount == frameCount)
        {
            return;
        }
        frameCount = currentFrameCount;
        animator.SetTrigger("Bounce");
        collisionParticles.Play();

    }
}
