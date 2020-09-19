using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitComponent : EntityComponent
{
    //Vector3 oldPosition;
    private Vector3 targetPosition;
    private Vector3 currentVelocity;
    private static float smoothMovementTime = 0.34f;
    public void Move( Hex newHex)
    {
        //animation
        this.transform.position = targetPosition;
        currentVelocity = Vector3.zero;
        targetPosition = newHex.PositionInWorld() + (1f * Vector3.up);

        //transform.position = newHex.PositionInWorld() + (1f * Vector3.up);
    }

    public void Move(Vector3[] path)
    {
        //animation
        this.transform.position = targetPosition;
        currentVelocity = Vector3.zero;
        // targetPosition = newHex.PositionInWorld() + (1f * Vector3.up);
        this.transform.position = path[path.Length - 1];
        //transform.position = newHex.PositionInWorld() + (1f * Vector3.up);
    }

    private void Start()
    {
        targetPosition = this.transform.position;
    }

    private void Update()
    {
        return;
        this.transform.position = Vector3.SmoothDamp
            (this.transform.position, targetPosition, ref currentVelocity, smoothMovementTime);
    }
}
