using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected Rigidbody rigidbody;
    protected bool isAlive = true;
    public void Die(Transform parent)
    {
        isAlive = false;
        transform.parent = parent;
        StartCoroutine(DeathSceneCoroutine());
        Debug.Log("Enemy down!");
    }

    private IEnumerator DeathSceneCoroutine()
    {
       
        yield return new WaitForSeconds(0.6f);
        gameObject.SetActive(false);
        EffectSpawner.SpawnEnemyDeathEffect(transform.position);

    }


    private void OnTriggerStay(Collider other)
    {

        if (GameManager.ABSTRACT_PLAYER && !GameManager.GameOver)
        {
            //TODO: probably ineffitient, try using bounds check
            if (other.transform.parent != null)
            {
                Hex hex = other.transform.parent.GetComponent<Hex>();
                if (hex != null)
                {
                                                                       // HexSpecialties hexSpecialty = hex.Specialty;
                    if (hex.State == HexStates.PotentiallyFull)
                    {
                        Debug.Log("An enemy is touching a sensitive hexagon...");
                        HexMap.InfectPlayerPath(hex);
                    }
                }
            }
        }
       
    }



}
