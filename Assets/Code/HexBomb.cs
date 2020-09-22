using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBomb : MonoBehaviour
{
    public Hex hex;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BallHexPainter>() != null)
        {

            HexMap.PrepareHexExplosion(hex);
            Destroy(gameObject);
        }
    }
}
