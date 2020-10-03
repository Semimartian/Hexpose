using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected Rigidbody rigidbody;
    protected bool isAlive = true;
    public bool IsAlive
    {
        get { return isAlive; }
    }

    public bool IsInterruptingFill
    {
        get { return capsuleCollider.enabled; }
    }

    [SerializeField] private Transform parentObject;
    [SerializeField] private CapsuleCollider capsuleCollider;

    private void Die(Transform parent)
    {
        isAlive = false;
        parentObject.parent = parent;
        StartCoroutine(DeathSceneCoroutine());
        Debug.Log("Enemy down!");
    }

    private IEnumerator DeathSceneCoroutine()
    {
        yield return new WaitForSeconds(Random.Range( 0.5f,0.78f));
        parentObject.gameObject.SetActive(false);
        EffectSpawner.SpawnEnemyDeathEffect(transform.position);
    }


    private void OnTriggerStay(Collider other)
    {

        if (GameManager.ABSTRACT_PLAYER && !GameManager.GameOver)
        {
            //TODO: probably ineffitient, try using bounds check
            //if (other.transform.parent != null)
            {
                Hex hex = other.transform/*.parent*/.GetComponent<Hex>();
                if (hex != null)
                {
                      // HexSpecialties hexSpecialty = hex.Specialty;
                    if (hex.State == HexStates.Path)
                    {
                        Debug.Log("An enemy is touching a sensitive hexagon...");
                        HexMap.InfectPlayerPath(hex);
                    }
                }
            }
        }
       
    }

    public void CheckForDeathFromBelow(bool dieByAnyHex)
    {
        if (!isAlive)
        {
            return;
        }

        List<Hex> hexesBelow = GetHexesBelow();
        int count = hexesBelow.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Hex hex = hexesBelow[i];

                if (hex.State == HexStates.Full || dieByAnyHex)
                {
                    Die(hex.transform);
                }
            }
        }
    }

     
    public List<Hex> GetHexesBelow()
    {
        List<Hex> hexes = new List<Hex>();
        Vector3 bottom = transform.position + (Vector3.up * -5);
        Vector3 top = transform.position;

        Collider[] collidersBelow =
            Physics.OverlapCapsule(bottom, top, capsuleCollider.radius*0.85f * transform.localScale.x);
        for (int i = 0; i < collidersBelow.Length; i++)
        {
            Collider collider = collidersBelow[i];
            Hex hex = collider.transform.GetComponent<Hex>();

            if (hex != null )
            {
                hexes.Add(hex);
            }
        }

        return hexes;
    } 
}
