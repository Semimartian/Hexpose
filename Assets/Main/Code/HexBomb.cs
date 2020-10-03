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
            Explode();
        }
    }

    public void Explode()
    {
        if(hex.State == HexStates.Full)
        {
            Debug.LogWarning("TriedToExplodeOnFullHex");
        }
        else
        {
            HexMap.PrepareHexExplosion(hex);

        }
        Destroy(gameObject);
    }
}
