using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrictRoamingEnemy : Enemy
{
    [SerializeField] private Transform[] positions;
    private int nextPositionIndex=0;

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private float maxSpeed;
    //[SerializeField] private Rigidbody rigidbody;

    void Start()
    {
        nextPositionIndex = 0;
        rigidbody.position = (positions[nextPositionIndex].position);
    }

    void FixedUpdate()
    {
        if(GameManager.GameState!= GameStates.GameOver && isAlive)
        {
            //I hate this. Lots of unssssry math;
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = positions[nextPositionIndex].position;
            Vector3 direction = (targetPosition - currentPosition).normalized;

            int previousPositionIndex = nextPositionIndex - 1;

            if (previousPositionIndex < 0)
            {
                previousPositionIndex = positions.Length - 1;
            }

            float curvePoint = 1 -
                 Vector3.Distance(currentPosition, targetPosition) /
                (Vector3.Distance(positions[previousPositionIndex].position, targetPosition));
            // Debug.Log("curvePoint: " + curvePoint);
            //Debug.Log("curveValueAtPoint: " + speedCurve.Evaluate(curvePoint));
            float currentSpeed =
                speedCurve.Evaluate(curvePoint) * maxSpeed * Time.fixedDeltaTime;
            Vector3 nextPosition = currentPosition + (direction * currentSpeed);
            rigidbody.position = (nextPosition);

            if ((nextPosition - targetPosition).magnitude < 0.1f)
            {
                nextPositionIndex++;
                if (nextPositionIndex >= positions.Length)
                {
                    nextPositionIndex = 0;
                }
            }
        }

    }
}
