using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    public Collider collider;
    public HexTypes hexType;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }
}
