using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedUpdateTest : MonoBehaviour
{
    private void FixedUpdate()
    {

        if(Random.Range(0,2) == 0)
        {
            transform.position = new Vector3(0, 45, 0);

        }
        else
        {
            transform.position = new Vector3(0, 35, 0);

        }
        float f = 1;
        for (int i = 0; i < Random.Range(0, 300); i++)
        {
            f *= 0.1f;
        }
        transform.position = new Vector3(0, 41, 0);

    }
}
