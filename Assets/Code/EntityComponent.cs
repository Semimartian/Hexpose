using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent : MonoBehaviour
{
    public void ChangeMaterial(Material material)
    {
        GetComponentInChildren<MeshRenderer>().material = material;
    }
}
