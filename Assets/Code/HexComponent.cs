using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexComponent : MonoBehaviour
{
    //public Hex hex;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public void UpdateGraphics(Vector3 positionInWorld,Material material, Mesh mesh)
    {
        transform.position = positionInWorld;
        //Debug.Log("UpdateGraphics");
        //meshRenderer.material = material;
       // meshFilter.mesh = mesh;
    }
   /* public void SetHex(Hex hex)
    {
        this.hex = hex;
    }*/
}
